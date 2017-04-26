using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
#if UNITY_EDITOR
	using UnityEditorInternal;
	using UnityEditor;
#endif

[DisallowMultipleComponent]
[ExecuteInEditMode]

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(Rigidbody))]

public class PortalManager : MonoBehaviour {
	//Public vars and correlated
	[Serializable] public class AutoPairClass {
		public bool Enable;

		public string PortalPairedName;
	}

	public AutoPairClass AutoPair;

	public GameObject PairedPortal;
	private bool ClearPairedPortal;
	public Material PairedPortalSkybox;
	[HideInInspector] public Material NullPairedPortalSkybox;

	[Serializable] public class ViewSettingsClass {
		[Serializable] public class ProjectionClass {
			public Vector2 Resolution = new Vector2(1280, 1024);

			public enum DepthQualityEnum {Fast, High};
			public DepthQualityEnum DepthQuality = DepthQualityEnum.High;
			[HideInInspector] public DepthQualityEnum CurrentDepthQuality;
		}
		public ProjectionClass Projection;

		[Serializable] public class RecursionClass {
			[Range(1, 20)] public int Steps = 1;
			public Material CustomFinalStep;
		}
		public RecursionClass Recursion;

		[Serializable] public class DistorsionClass {
			public bool EnableDistorsion;

			public Texture2D Pattern;
			public Color Color = new Color(1, 1, 1, 1);
			[Range(1, 100)] public int Tiling = 1;
			[Range(-10, 10)] public float SpeedX = .01f;
			[Range(-10, 10)] public float SpeedY = 0;
		}
		public DistorsionClass Distorsion;
	}
	public ViewSettingsClass ViewSettings;

	[Serializable] public class PortalSettingsClass {
		public bool EnableRender = true;
		public bool EnableObjsTrigger = true;
		public bool EnableClipPlane = true;
	}
	public PortalSettingsClass PortalSettings;

	[Serializable] public class PortalFunctionalityClass {
		[Serializable] public class ExcludedObjsFromTriggerClass {
			public GameObject Obj;

			public bool OnlyForPortal;
		}

		public ExcludedObjsFromTriggerClass[] ExcludedObjsFromTrigger = new ExcludedObjsFromTriggerClass[0];

		[Serializable] public class ExcludedObjsFromRenderClass {
			public GameObject Obj;

			[Range(2, 31)] public int Layer = 2;
		}
		public ExcludedObjsFromRenderClass[] ExcludedObjsFromRender = new ExcludedObjsFromRenderClass[0];

		[Serializable] public class SceneAsyncLoadClass {
			public bool Enable;

			public int SceneIndex = 0;
		}
		public SceneAsyncLoadClass SceneAsyncLoad;
	}
	public PortalFunctionalityClass PortalFunctionality;
	//----------

	private Material[] PortalMaterials;
	private RenderTexture[] RenderTexts;
	private Material ClipPlaneMaterial;
	private Material CloneClipPlaneMaterial;
	[HideInInspector] public GameObject ClipPlanePosObj;
	private Vector2 CurrentProjectionResolution;
	[HideInInspector] public GameObject[] PortalCamObjs;
	private int[] InitPortalCamObjsCullingMask;
	private GameObject SceneviewRender;

	void OnEnable () {
		#if UNITY_EDITOR
			RenderTexts = new RenderTexture [2];
		#else
			RenderTexts = new RenderTexture [1];
		#endif
		PortalMaterials = new Material [RenderTexts.Length];

		PortalCamObjs = new GameObject [20];
		Array.Resize (ref PortalCamObjs, PortalCamObjs.Length + 1);
		InitPortalCamObjsCullingMask = new int [PortalCamObjs.Length];

		for (int i = 0; i < PortalMaterials.Length; i++) //Generate "Portal" and "Clipping plane" materials
			if (!PortalMaterials [i])
				PortalMaterials [i] = new Material (Shader.Find ("Gater/UV Remap"));

		Shader ClipPlaneShader = Shader.Find ("Custom/StandardClippable");

		if (!NullPairedPortalSkybox)
			NullPairedPortalSkybox = new Material (Shader.Find ("Standard"));

		if (!ClipPlaneMaterial) {
			ClipPlaneMaterial = new Material (Shader.Find ("Standard"));
			ClipPlaneMaterial.shader = ClipPlaneShader;
		}
		if (!CloneClipPlaneMaterial) {
			CloneClipPlaneMaterial = new Material (Shader.Find ("Standard"));
			CloneClipPlaneMaterial.shader = ClipPlaneShader;
		}

		//Apply custom settings to the portal components
		GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		GetComponent<MeshRenderer> ().receiveShadows = false;
		GetComponent<MeshRenderer> ().sharedMaterial = PortalMaterials [0];
		GetComponent<Rigidbody> ().mass = 1;
		GetComponent<Rigidbody> ().drag = 0;
		GetComponent<Rigidbody> ().angularDrag = 0;
		GetComponent<Rigidbody> ().useGravity = false;
		GetComponent<Rigidbody> ().isKinematic = true;
		GetComponent<Rigidbody> ().interpolation = RigidbodyInterpolation.None;
		GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		if (GetComponent<MeshCollider> ()) {
			GetComponent<MeshCollider> ().convex = true;
			GetComponent<MeshCollider> ().sharedMaterial = null;
		}

		//Disable collision of walls behind portals, and check if the excluded objects from trigger have a collider component
		if (PortalFunctionality.ExcludedObjsFromTrigger.Length > 0)
			for (int j = 0; j < PortalFunctionality.ExcludedObjsFromTrigger.Length; j++)
				if (PortalFunctionality.ExcludedObjsFromTrigger [j].Obj) {
					Physics.IgnoreCollision (transform.GetComponent<Collider> (), PortalFunctionality.ExcludedObjsFromTrigger [j].Obj.GetComponent<Collider> (), true);

					if (!PortalFunctionality.ExcludedObjsFromTrigger [j].Obj.GetComponent<Collider> ())
						Debug.LogError ("One excluded wall doesn't have a collider component");
				}

		#if UNITY_EDITOR
			EditorApplication.update = null;
			
			//Search already existing required objects for teleport, and fill the relative variables with
			int PortalCamObjsSteps = 0;

			for (int k = 0; k < transform.GetComponentsInChildren<Transform> ().Length; k++) {
				if (transform.GetComponentsInChildren<Transform> () [k].name == this.gameObject.name + " Camera " + PortalCamObjsSteps) {
					PortalCamObjs [PortalCamObjsSteps] = transform.GetComponentsInChildren<Transform> () [k].gameObject;

					PortalCamObjsSteps += 1;
				}

				if (transform.GetComponentsInChildren<Transform> () [k].name == transform.name + " SceneviewRender")
					SceneviewRender = transform.GetComponentsInChildren<Transform> () [k].gameObject;

				if (transform.GetComponentsInChildren<Transform> () [k].name == transform.name + " ClipPlanePosObj")
					ClipPlanePosObj = transform.GetComponentsInChildren<Transform> () [k].gameObject;
			}
		#endif
	}

	private Mesh PortalMesh;
	private Camera InGameCamera;
	private RenderTexture TempRenderText;

	void LateUpdate () {
		if (!PairedPortal || PairedPortal.GetComponent<PortalManager> ().PairedPortal != gameObject) {
			if (ClearPairedPortal) {
				PairedPortal = null;

				ClearPairedPortal = false;
			} else
				if (AutoPair.Enable) {
					if (AutoPair.PortalPairedName != "")
						PairedPortal = GameObject.Find (AutoPair.PortalPairedName);
				} else
					for (int i = 0; i < FindObjectsOfType<Transform> ().Length; i++)
						if (FindObjectsOfType<Transform> () [i] != transform && FindObjectsOfType<Transform> () [i].GetComponent<PortalManager> () && FindObjectsOfType<Transform> () [i].GetComponent<PortalManager> ().PairedPortal == gameObject)
							PairedPortal = FindObjectsOfType<Transform> () [i].gameObject;
		} else {
			if (!InGameCamera) {
				ResetVars (false); //Reset arrays elements of the required objects for teleport, if game camera variable is null and any object is still colliding with the portal

				InGameCamera = Camera.main; //Fill empty "InGameCamera" variable with main camera
			} else {
				ClearPairedPortal = true;

				if (InGameCamera.nearClipPlane > .01f)
					Debug.LogError ("The nearClipPlane of 'Main Camera' is not equal to 0.01");

				PortalMesh = GetComponent<MeshFilter> ().sharedMesh; //Acquire current portal mesh

				ViewSettings.Projection.Resolution = new Vector2 (ViewSettings.Projection.Resolution.x < 1 ? 1 : ViewSettings.Projection.Resolution.x, ViewSettings.Projection.Resolution.y < 1 ? 1 : ViewSettings.Projection.Resolution.y);

				//Generate render texture for the portal camera
				if (CurrentProjectionResolution.x != ViewSettings.Projection.Resolution.x || CurrentProjectionResolution.y != ViewSettings.Projection.Resolution.y || ViewSettings.Projection.CurrentDepthQuality != ViewSettings.Projection.DepthQuality) {
					if (TempRenderText) {
						#if UNITY_EDITOR
							DestroyImmediate (TempRenderText, false);
						#else
							Destroy (TempRenderText);
						#endif
					} else
						TempRenderText = new RenderTexture (Convert.ToInt32 (ViewSettings.Projection.Resolution.x), Convert.ToInt32 (ViewSettings.Projection.Resolution.y), ViewSettings.Projection.DepthQuality == ViewSettingsClass.ProjectionClass.DepthQualityEnum.Fast ? 16 : 24);

					int RenderTextsLength = 0;

					for (int i = 0; i < RenderTexts.Length; i++) {
						if (RenderTexts [i]) {
							#if UNITY_EDITOR
								if (!EditorApplication.isPlaying)
									DestroyImmediate (RenderTexts [i], false);
								if (EditorApplication.isPlaying)
									Destroy (RenderTexts [i]);
							#else
								Destroy (RenderTexts [i]);
							#endif
						} else {
							RenderTexts [i] = new RenderTexture (Convert.ToInt32 (ViewSettings.Projection.Resolution.x), Convert.ToInt32 (ViewSettings.Projection.Resolution.y), ViewSettings.Projection.DepthQuality == ViewSettingsClass.ProjectionClass.DepthQualityEnum.Fast ? 16 : 24);
							RenderTexts [i].name = this.gameObject.name + " RenderTexture " + i;

							RenderTextsLength += 1;
						}
					}

					if (RenderTextsLength == RenderTexts.Length) {
						CurrentProjectionResolution = new Vector2 (ViewSettings.Projection.Resolution.x, ViewSettings.Projection.Resolution.y);

						ViewSettings.Projection.CurrentDepthQuality = ViewSettings.Projection.DepthQuality;
					}
				}

				#if UNITY_EDITOR
					LayerMask SceneTabLayerMask = Tools.visibleLayers;

					SceneTabLayerMask &= ~(1 << 1); //Disable SceneviewRender layer on Sceneview

					Tools.visibleLayers = SceneTabLayerMask;

					//Generate projection plane for Sceneview
					if (!SceneviewRender) {
						SceneviewRender = new GameObject (transform.name + " SceneviewRender");

						SceneviewRender.AddComponent<MeshFilter> ();
						SceneviewRender.AddComponent<MeshRenderer> ();

						SceneviewRender.transform.position = transform.position;
						SceneviewRender.transform.rotation = transform.rotation;
						SceneviewRender.transform.localScale = transform.localScale;
						SceneviewRender.transform.parent = transform;
					} else {
						if (SceneviewRender.name != transform.name + " SceneviewRender")
							SceneviewRender.name = transform.name + " SceneviewRender";

						SceneviewRender.layer = 4;

						SceneviewRender.transform.localPosition = new Vector3 (0, 0, .0001f);

						SceneviewRender.GetComponent<MeshFilter> ().sharedMesh = PortalMesh;
						SceneviewRender.GetComponent<MeshRenderer> ().sharedMaterial = PortalMaterials [1];
						//Enable/Disable portal rendering by bool variable
						SceneviewRender.GetComponent<MeshRenderer> ().enabled = PortalSettings.EnableRender ? true : false;
						
						//Apply render texture to the scene portal material
						if (PortalMaterials.Length > 1)
							SceneviewRender.GetComponent<MeshRenderer> ().sharedMaterial.SetTexture ("_MainTex", InGameCamera && PairedPortal.GetComponent<PortalManager> ().RenderTexts [1] ? PairedPortal.GetComponent<PortalManager> ().RenderTexts [1] : null);
					}
				#endif

				//Apply render texture to the game portal material
				if (PortalMaterials.Length > 0)
					GetComponent<MeshRenderer> ().sharedMaterial.SetTexture ("_MainTex", InGameCamera && PairedPortal.GetComponent<PortalManager> ().RenderTexts [0] ? PairedPortal.GetComponent<PortalManager> ().RenderTexts [0] : null);

				//Enable/Disable portal rendering by bool variable
				GetComponent<MeshRenderer> ().enabled = PortalSettings.EnableRender ? true : false;
				//Manage distorstion pattern settings
				GetComponent<MeshRenderer> ().sharedMaterial.SetInt ("_EnableDistorsionPattern", ViewSettings.Distorsion.EnableDistorsion ? 1 : 0);
				GetComponent<MeshRenderer> ().sharedMaterial.SetTexture ("_DistorsionPattern", ViewSettings.Distorsion.Pattern);
				GetComponent<MeshRenderer> ().sharedMaterial.SetColor ("_DistorsionPatternColor", ViewSettings.Distorsion.Color);
				GetComponent<MeshRenderer> ().sharedMaterial.SetInt ("_DistorsionPatternTiling", ViewSettings.Distorsion.Tiling);
				GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_DistorsionPatternSpeedX", -ViewSettings.Distorsion.SpeedX);
				GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_DistorsionPatternSpeedY", -ViewSettings.Distorsion.SpeedY);

				//Generate camera for the portal rendering
				for (int j = 0; j < PortalCamObjs.Length; j++) {
					if (j < ViewSettings.Recursion.Steps + 1) {
						if (!PortalCamObjs [j]) {
							PortalCamObjs [j] = new GameObject (transform.name + " Camera " + j);

							PortalCamObjs [j].tag = "Untagged";

							PortalCamObjs [j].transform.parent = transform;
							PortalCamObjs [j].AddComponent<Camera> ();
							PortalCamObjs [j].GetComponent<Camera> ().enabled = false;
							InitPortalCamObjsCullingMask [j] = PortalCamObjs [j].GetComponent<Camera> ().cullingMask;
							PortalCamObjs [j].GetComponent<Camera> ().nearClipPlane = .01f;

							PortalCamObjs [j].AddComponent<Skybox> ();
						} else {
							if (PortalCamObjs [j].name != transform.name + " Camera " + j)
								PortalCamObjs [j].name = transform.name + " Camera " + j;

							if (PortalCamObjs [j].GetComponent<Camera> ().depth != InGameCamera.depth - 1)
								PortalCamObjs [j].GetComponent<Camera> ().depth = InGameCamera.depth - 1;

							//Acquire settings from Scene/Game camera, to apply on Portal camera
							if (InGameCamera) {
								PortalCamObjs [j].GetComponent<Camera> ().renderingPath = InGameCamera.renderingPath;
								PortalCamObjs [j].GetComponent<Camera> ().useOcclusionCulling = InGameCamera.useOcclusionCulling;
								PortalCamObjs [j].GetComponent<Camera> ().hdr = InGameCamera.hdr;
							}
						}

						if (PairedPortal.GetComponent<PortalManager> ().PortalCamObjs [j])
							PairedPortal.GetComponent<PortalManager> ().PortalCamObjs [j].GetComponent<Skybox> ().material = ViewSettings.Recursion.CustomFinalStep && (j > 0 && j == ViewSettings.Recursion.Steps) ? ViewSettings.Recursion.CustomFinalStep : (!PairedPortalSkybox && (j > 0 && j == ViewSettings.Recursion.Steps) ? NullPairedPortalSkybox : PairedPortalSkybox);
					} else {
						#if UNITY_EDITOR
							if (!EditorApplication.isPlaying)
								DestroyImmediate (PortalCamObjs [j], false);
							if (EditorApplication.isPlaying)
								Destroy (PortalCamObjs [j]);
						#else
							Destroy (PortalCamObjs [j]);
						#endif
					}
				}

				//Generate mesh clip plane modificator object
				if (!ClipPlanePosObj) {
					ClipPlanePosObj = new GameObject (transform.name + " ClipPlanePosObj");

					ClipPlanePosObj.transform.position = transform.position;
					ClipPlanePosObj.transform.rotation = transform.rotation;
					ClipPlanePosObj.transform.parent = transform;
				} else {
					ClipPlanePosObj.transform.localPosition = new Vector3 (0, 0, .005f);

					if (ClipPlanePosObj.name != transform.name + " ClipPlanePosObj")
						ClipPlanePosObj.name = transform.name + " ClipPlanePosObj";
				}

				gameObject.layer = 1;

				//Apply current portal mesh to the mesh collider if exist
				if (GetComponent<MeshCollider> () && GetComponent<MeshCollider> ().sharedMesh != PortalMesh)
					GetComponent<MeshCollider> ().sharedMesh = PortalMesh;
				//Disable trigger of portal collider
				if (GetComponent<Collider> ()) {
					if (GetComponent<Collider> ().isTrigger != (InGameCamera ? true : false))
						GetComponent<Collider> ().isTrigger = InGameCamera ? (PortalSettings.EnableObjsTrigger ? true : false) : false;
				} else
					Debug.LogError ("No collider component found");

				//Move portal cameras and render it to rendertexture
				if (PairedPortal) {
					Vector3[] PortalCamPos = new Vector3[PortalCamObjs.Length];
					Quaternion[] PortalCamRot = new Quaternion[PortalCamObjs.Length];

					if (PairedPortal.GetComponent<PortalManager> ().PortalSettings.EnableRender) {
						for (int i = 0; i < RenderTexts.Length; i++) {
							if (RenderTexts [i]) {
								for (int j = ViewSettings.Recursion.Steps; j >= 0; j--) {
									if (PortalCamObjs [j]) {
										//Move portal camera to position/rotation of Scene/Game camera
										Camera SceneCamera = null;

										#if UNITY_EDITOR
											SceneCamera = SceneView.GetAllSceneCameras ().Length > 0 ? SceneView.GetAllSceneCameras () [0] : null;
										#endif

										PortalCamObjs [j].GetComponent<Camera> ().aspect = (i == 1 && SceneCamera ? SceneCamera.aspect : InGameCamera.aspect);
										PortalCamObjs [j].GetComponent<Camera> ().fieldOfView = (i == 1 && SceneCamera ? SceneCamera.fieldOfView : InGameCamera.fieldOfView);
										PortalCamObjs [j].GetComponent<Camera> ().farClipPlane = (i == 1 && SceneCamera ? SceneCamera.farClipPlane : InGameCamera.farClipPlane);

										PortalCamPos [j] = PairedPortal.transform.InverseTransformPoint (i == 1 && SceneCamera ? SceneCamera.transform.position : InGameCamera.transform.position);

										PortalCamPos [j].x = -PortalCamPos [j].x;
										PortalCamPos [j].z = -PortalCamPos [j].z + j * (Vector3.Distance (transform.position, PairedPortal.transform.position) / 5);

										PortalCamRot [j] = Quaternion.Inverse (PairedPortal.transform.rotation) * (i == 1 && SceneCamera ? SceneCamera.transform.rotation : InGameCamera.transform.rotation);

										PortalCamRot [j] = Quaternion.AngleAxis (180.0f, new Vector3 (0, 1, 0)) * PortalCamRot [j];

										PortalCamObjs [j].transform.localPosition = PortalCamPos [j];
										PortalCamObjs [j].transform.localRotation = PortalCamRot [j];

										//Render inside portal cameras to render texture
										if (j > 0 && j == ViewSettings.Recursion.Steps)
											PortalCamObjs [j].GetComponent<Camera> ().cullingMask = 0;
										else {
											PortalCamObjs [j].GetComponent<Camera> ().cullingMask = InGameCamera.cullingMask;

											for (int k = 0; k < PortalFunctionality.ExcludedObjsFromRender.Length; k++)
												if (PortalFunctionality.ExcludedObjsFromRender [k].Obj)
													PortalCamObjs [j].GetComponent<Camera> ().cullingMask &= ~(1 << PortalFunctionality.ExcludedObjsFromRender [k].Layer);

											if (i == 0)
												PortalCamObjs [j].GetComponent<Camera> ().cullingMask &= ~(1 << 4);
											else
												PortalCamObjs [j].GetComponent<Camera> ().cullingMask &= ~(1 << 1);
										}

										PortalCamObjs [j].GetComponent<Camera> ().targetTexture = TempRenderText;

										PortalCamObjs [j].GetComponent<Camera> ().Render ();

										Graphics.Blit (TempRenderText, RenderTexts [i]);

										PortalCamObjs [j].GetComponent<Camera> ().targetTexture = null;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	class InitMaterialsList { public Material[] Materials; }
	private GameObject[] CollidedObjs = new GameObject[0];
	private string[] CollidedObjsInitName = new string[0];
	[HideInInspector] public Vector3 CollidedObjsParentPreviousFirstPos;
	[HideInInspector] public Vector3 CollidedObjsParentPreviousSecondPos;
	private bool AcquireNextPos;
	private InitMaterialsList[] CollidedObjsInitMaterials = new InitMaterialsList[0];
	private bool[] StandardObjShader = new bool[0];
	private bool[] CollidedObjsAlwaysTeleport = new bool[0];
	private bool[] CollidedObjsFirstTrig = new bool[0];
	private float[] CollidedObjsFirstTrigDist = new float[0];
	private GameObject[] ProxDetCollidedObjs = new GameObject[0];
	private GameObject[] CloneCollidedObjs = new GameObject[0];
	private Vector3[] CollidedObjVelocity = new Vector3[0];
	private bool[] ContinueTriggerEvents = new bool[0];
	[HideInInspector] public bool CollidedObjsExternalParent;
	[HideInInspector] public int EnterTriggerTimes;

	void OnTriggerEnter (Collider collision) {
		//Disable collision with objects excluded from trigger during teleport
		if (PortalFunctionality.ExcludedObjsFromTrigger.Length > 0)
			for (int i = 0; i < PortalFunctionality.ExcludedObjsFromTrigger.Length; i++)
				if (collision.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () && !PortalFunctionality.ExcludedObjsFromTrigger [i].OnlyForPortal)
					Physics.IgnoreCollision (collision, PortalFunctionality.ExcludedObjsFromTrigger [i].Obj.GetComponent<Collider> (), true);

		//Increment and partially fill the arrays elements of required object for teleport
		if (collision.gameObject != this.gameObject && !collision.GetComponent<PortalManager> () && !collision.name.Contains (collision.gameObject.GetHashCode ().ToString ()) && !collision.name.Contains ("Clone")) {
			Array.Resize (ref CollidedObjs, CollidedObjs.Length + 1);
			Array.Resize (ref CollidedObjsInitName, CollidedObjsInitName.Length + 1);
			Array.Resize (ref CollidedObjsInitMaterials, CollidedObjsInitMaterials.Length + 1);
			Array.Resize (ref StandardObjShader, StandardObjShader.Length + 1);
			Array.Resize (ref CollidedObjsAlwaysTeleport, CollidedObjsAlwaysTeleport.Length + 1);
			Array.Resize (ref CollidedObjsFirstTrig, CollidedObjsFirstTrig.Length + 1);
			Array.Resize (ref CollidedObjsFirstTrigDist, CollidedObjsFirstTrigDist.Length + 1);
			Array.Resize (ref ProxDetCollidedObjs, ProxDetCollidedObjs.Length + 1);
			Array.Resize (ref CloneCollidedObjs, CloneCollidedObjs.Length + 1);
			Array.Resize (ref CollidedObjVelocity, CollidedObjVelocity.Length + 1);
			Array.Resize (ref ContinueTriggerEvents, ContinueTriggerEvents.Length + 1);

			CollidedObjs [CollidedObjs.Length - 1] = collision.gameObject;
			CollidedObjsInitName [CollidedObjsInitName.Length - 1] = collision.gameObject.name;
			if (!collision.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ()) {
				if (collision.transform.childCount > 0) {
					EnterTriggerTimes = 0;
					PairedPortal.GetComponent<PortalManager> ().EnterTriggerTimes = 0;

					for (int j = 0; j < collision.GetComponentsInChildren<Transform> ().Length; j++)
						if (collision.GetComponentsInChildren<Transform> () [j] != collision.gameObject && collision.GetComponentsInChildren<Transform> () [j].GetComponent<Camera> ()) {
							CollidedObjsParentPreviousFirstPos = collision.GetComponentsInChildren<Transform> () [j].transform.localPosition;
							PairedPortal.GetComponent<PortalManager> ().CollidedObjsParentPreviousFirstPos = collision.GetComponentsInChildren<Transform> () [j].transform.localPosition;

							AcquireNextPos = true;
							PairedPortal.GetComponent<PortalManager> ().AcquireNextPos = true;
						}
				}
				if (!AcquireNextPos && collision.GetComponent<Camera> ()) {
					EnterTriggerTimes += 1;
					PairedPortal.GetComponent<PortalManager> ().EnterTriggerTimes += 1;

					if (EnterTriggerTimes == 2)
						CollidedObjsExternalParent = false;
				}
				if (AcquireNextPos) {
					if (collision.transform.childCount == 0 && collision.gameObject && collision.GetComponent<Camera> ()) {
						CollidedObjsParentPreviousSecondPos = collision.transform.localPosition;
						PairedPortal.GetComponent<PortalManager> ().CollidedObjsParentPreviousSecondPos = collision.transform.localPosition;

						AcquireNextPos = false;
						PairedPortal.GetComponent<PortalManager> ().AcquireNextPos = false;
					}
				}
			}

			if (CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ()) {
				if (!StandardObjShader[CollidedObjs.Length - 1]) {
					if (CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterial.shader.name != "Standard")
						Debug.LogError ("The shader of object material is not 'Standard', mesh clippping will not be possible");
					if (CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterial.shader.name == "Standard")
						StandardObjShader[CollidedObjs.Length - 1] = true;
				}
				if (StandardObjShader[CollidedObjs.Length - 1]) {
					if (CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> () && PortalSettings.EnableClipPlane) {
						CollidedObjsInitMaterials [CollidedObjsInitMaterials.Length - 1] = new InitMaterialsList ();

						CollidedObjsInitMaterials [CollidedObjsInitMaterials.Length - 1].Materials = CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterials;

						CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterial = ClipPlaneMaterial;
						CollidedObjs [CollidedObjs.Length - 1].GetComponent<MeshRenderer> ().sharedMaterial.CopyPropertiesFromMaterial (CollidedObjsInitMaterials [CollidedObjs.Length - 1].Materials [0]);
					}
				}
			}

			ContinueTriggerEvents [ContinueTriggerEvents.Length - 1] = true;
		}
	}

	private GameObject[] ObjCollidedCamObj = new GameObject[2];
	private GameObject[] ObjCloneCollidedCamObj = new GameObject[2];

	void OnTriggerStay (Collider collision) {
		//Change position/rotation of required objects for teleport, and complete the fill of remaining arrays elements
		for (int i = 0; i < CollidedObjs.Length; i++) {
			if (ContinueTriggerEvents [i] && CollidedObjs [i]) {
				if (!ProxDetCollidedObjs [i]) {
					ProxDetCollidedObjs [i] = new GameObject (CollidedObjs [i].name + " Proximity Detector");

					ProxDetCollidedObjs [i].transform.position = transform.position;
					ProxDetCollidedObjs [i].transform.rotation = transform.rotation;
					ProxDetCollidedObjs [i].transform.parent = transform;
				}
				if (ProxDetCollidedObjs [i]) {
					if (PortalMesh && ProxDetCollidedObjs [i]) {
						if (CollidedObjs [i].transform.childCount > 0) {
							for (int j = 0; j < CollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; j++)
								if (CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () && CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].GetComponent<Camera> ())
									ObjCollidedCamObj [0] = CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].gameObject;
						}
						if (CollidedObjs [i].transform.childCount == 0)
						if (CollidedObjs [i].GetComponent<Camera> ())
							ObjCollidedCamObj [1] = CollidedObjs [i];

						Vector3 ProxDetCollidedObjPos = transform.InverseTransformPoint ((ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0] ? ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].transform.position : CollidedObjs [i].transform.position));
						Vector3 ProxDetCollLimit = new Vector3 (PortalMesh.bounds.size.x / 2, PortalMesh.bounds.size.y / 2, PortalMesh.bounds.size.z / 2);

						ProxDetCollidedObjs [i].transform.localPosition = new Vector3 (ProxDetCollidedObjPos.x > -ProxDetCollLimit.x && ProxDetCollidedObjPos.x < ProxDetCollLimit.x ? ProxDetCollidedObjPos.x : ProxDetCollidedObjs [i].transform.localPosition.x, ProxDetCollidedObjPos.y > -ProxDetCollLimit.y && ProxDetCollidedObjPos.y < ProxDetCollLimit.y ? ProxDetCollidedObjPos.y : ProxDetCollidedObjs [i].transform.localPosition.y, ProxDetCollidedObjs [i].transform.localPosition.z);

						if (!CollidedObjsAlwaysTeleport [i]) {
							if (!CollidedObjsFirstTrig [i]) {
								CollidedObjsFirstTrigDist [i] = Vector3.Dot (CollidedObjs [i].transform.position - ProxDetCollidedObjs [i].transform.position, ProxDetCollidedObjs [i].transform.forward);

								CollidedObjsFirstTrig [i] = true;
							}
							if (CollidedObjsFirstTrig [i] && CollidedObjsFirstTrigDist [i] < 0) {
								CollidedObjsAlwaysTeleport [i] = true;

								CollidedObjs [i].name = CollidedObjs [i].name + " " + CollidedObjs [i].GetHashCode ().ToString ();
							}
						}
						if (CollidedObjsAlwaysTeleport [i]) {
							if (!CloneCollidedObjs [i]) {
								CloneCollidedObjs [i] = (GameObject)Instantiate (CollidedObjs [i], PairedPortal.transform.position, PairedPortal.transform.rotation);

								if (CloneCollidedObjs [i].GetComponent<AudioListener> ())
									Destroy (CloneCollidedObjs [i].GetComponent<AudioListener> ());

								if (CloneCollidedObjs [i].transform.childCount > 0) {
									for (int k = 0; k < CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; k++) {
										if (CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k] != CloneCollidedObjs [i].transform && CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<MeshRenderer> ())
											CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<MeshRenderer> ().enabled = false;

										if (CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<AudioListener> ())
											Destroy (CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<AudioListener> ());

										if (CloneCollidedObjs [i].transform.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () && CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<Camera> ())
											ObjCloneCollidedCamObj [0] = CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].gameObject;
										if (!CloneCollidedObjs [i].transform.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () && CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<Camera> ())
											CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].gameObject.GetComponent<Camera> ().enabled = false;
									}
								}
								if (CloneCollidedObjs [i].transform.childCount == 0)
								if (CloneCollidedObjs [i].GetComponent<Camera> ())
									ObjCloneCollidedCamObj [1] = CloneCollidedObjs [i];

								if (CloneCollidedObjs [i].GetComponent<MeshRenderer> () && StandardObjShader [i]) {
									if (PortalSettings.EnableClipPlane) {
										CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial = CloneClipPlaneMaterial;
										CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.CopyPropertiesFromMaterial (CollidedObjsInitMaterials [i].Materials [0]);
									}
									if (!PortalSettings.EnableClipPlane) {
										CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterials = CollidedObjsInitMaterials [i].Materials;
										CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterials = CollidedObjsInitMaterials [i].Materials;
									}
								}

								if (ObjCloneCollidedCamObj [0]) {
									CloneCollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().enabled = false;

									CloneCollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_OriginalCameraPosition = CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_OriginalCameraPosition;
								}

								CloneCollidedObjs [i].name = CollidedObjsInitName [i] + " Clone";

								CloneCollidedObjs [i].transform.position = PairedPortal.transform.position;
								CloneCollidedObjs [i].transform.parent = PairedPortal.transform;
							}
							if (CloneCollidedObjs [i]) {
								float DistAmount = .007f;

								float CollidedObjProxDetDistStay = Vector3.Dot ((ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0] ? ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].transform.position : CollidedObjs [i].transform.position) - ProxDetCollidedObjs [i].transform.position, ProxDetCollidedObjs [i].transform.forward);
								Vector3 CloneCollidedObjLocalPos = transform.InverseTransformPoint (CollidedObjs [i].transform.position);

								CloneCollidedObjLocalPos.x = -CloneCollidedObjLocalPos.x;
								CloneCollidedObjLocalPos.z = -CloneCollidedObjLocalPos.z - DistAmount;

								CloneCollidedObjs [i].transform.localPosition = CloneCollidedObjLocalPos;

								Quaternion CloneCollidedObjLocalRot = Quaternion.Inverse (transform.rotation) * (CollidedObjs [i].transform.rotation);

								CloneCollidedObjLocalRot = Quaternion.AngleAxis (180.0f, new Vector3 (0, -1, 0)) * CloneCollidedObjLocalRot;

								CloneCollidedObjs [i].transform.localRotation = CloneCollidedObjLocalRot;

								if (ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0] && ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0]) {
									if (!ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> ())
										ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].AddComponent<Skybox> ();
									if (ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> ()) {
										ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> ().material = PairedPortalSkybox;

										if (!PairedPortalSkybox) {
											#if UNITY_EDITOR
												if (!EditorApplication.isPlaying)
													DestroyImmediate (ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> (), false);
												if (EditorApplication.isPlaying)
													Destroy (ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> ());
											#else
												Destroy (ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> ());
											#endif
										}
									}

									ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].GetComponent<Camera> ().enabled = CollidedObjProxDetDistStay < -DistAmount ? true : false;
									ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].GetComponent<Camera> ().enabled = CollidedObjProxDetDistStay >= -DistAmount ? true : false;

									InGameCamera = ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].GetComponent<Camera> ().enabled ? ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].GetComponent<Camera> () : ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].GetComponent<Camera> ();

									if (ObjCloneCollidedCamObj [0]) {
										ObjCloneCollidedCamObj [0].transform.localPosition = ObjCollidedCamObj [0].transform.localPosition;
										ObjCloneCollidedCamObj [0].transform.localRotation = ObjCollidedCamObj [0].transform.localRotation;
									}
								}

								if (CollidedObjs [i].GetComponent<MeshRenderer> () && CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial == ClipPlaneMaterial) {
									Vector3 DirectionVector = Vector3.forward;

									CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.SetVector ("_planePos", ClipPlanePosObj.transform.position);
									CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.SetVector ("_planeNorm", Quaternion.Euler (transform.eulerAngles) * -DirectionVector);

									CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.SetVector ("_planePos", PairedPortal.GetComponent<PortalManager> ().ClipPlanePosObj.transform.position);
									CloneCollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterial.SetVector ("_planeNorm", Quaternion.Euler (PairedPortal.transform.eulerAngles) * -DirectionVector);
								}
							}
						}
					}
				}
			}
		}
	}

	private Vector3 PreviousCollidedObjsInternalParentPos;

	void OnTriggerExit (Collider collision) {
		//Destroy required objects for teleport, reset relative arrays, and move original collided object to the its final position/rotation
		for (int i = 0; i < CloneCollidedObjs.Length; i++) {
			if (ContinueTriggerEvents [i] && CollidedObjs [i] && CollidedObjs [i].GetHashCode ().ToString () == collision.gameObject.GetHashCode ().ToString () && CloneCollidedObjs [i]) {
				if (CollidedObjVelocity [i] == Vector3.zero)
					CollidedObjVelocity [i] = CollidedObjs [i].GetComponent<Rigidbody> () ? CollidedObjs [i].GetComponent<Rigidbody> ().velocity.magnitude * -PairedPortal.transform.forward : new Vector3 (0, 0, 0);

				float CollObjProxDetDistExit = Vector3.Dot (CollidedObjs [i].transform.position - ProxDetCollidedObjs [i].transform.position, ProxDetCollidedObjs [i].transform.forward);

				GameObject[] CollidedObjsInternalParent = new GameObject[CollidedObjs [i].transform.childCount > 0 && !CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () ? CollidedObjs [i].transform.childCount : 0];

				if (CollidedObjsInternalParent.Length > 0) {
					for (int j = 0; j < CollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; j++) {
						if (CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j] != CollidedObjs [i].transform && CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].GetComponent<Camera> ()) {
							CollidedObjsInternalParent [0] = CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].gameObject;
							PreviousCollidedObjsInternalParentPos = CollidedObjsInternalParent [0].transform.position;
						}
					}
				}

				if (CollidedObjs [i].transform.childCount > 0) {
					for (int k = 0; k < CollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; k++)
						if (CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () && CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<Camera> ())
							ObjCollidedCamObj [0] = CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].gameObject;
				}
				if (CollidedObjs [i].transform.childCount == 0)
				if (CollidedObjs [i].GetComponent<Camera> ())
					ObjCollidedCamObj [1] = CollidedObjs [i];
				if (CloneCollidedObjs [i].transform.childCount > 0) {
					for (int k = 0; k < CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; k++)
						if (CloneCollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () && CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].GetComponent<Camera> ())
							ObjCloneCollidedCamObj [0] = CloneCollidedObjs [i].transform.GetComponentsInChildren<Transform> () [k].gameObject;
				}
				if (CloneCollidedObjs [i].transform.childCount == 0)
				if (CloneCollidedObjs [i].GetComponent<Camera> ())
					ObjCloneCollidedCamObj [1] = CloneCollidedObjs [i];

				if (CollObjProxDetDistExit > 0) {
					if (!CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ()) {
						if (CollidedObjsInternalParent.Length == 0) {
							if (CollidedObjs [i].transform.parent != null) {
								if (!CollidedObjsExternalParent)
									CollidedObjs [i].transform.localPosition = CollidedObjsParentPreviousFirstPos;
								if (CollidedObjsExternalParent)
									CollidedObjs [i].transform.localPosition = CollidedObjsParentPreviousSecondPos;
							}
							if (CollidedObjs [i].transform.parent == null)
								CollidedObjs [i].transform.position = CloneCollidedObjs [i].transform.position;
						}
						if (CollidedObjsInternalParent.Length > 0) {
							CollidedObjs [i].transform.position = CloneCollidedObjs [i].transform.position;

							CollidedObjsInternalParent [0].transform.position = PreviousCollidedObjsInternalParentPos;
						}
					}
					if (CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ())
						CollidedObjs [i].transform.position = CloneCollidedObjs [i].transform.position;

					CollidedObjs [i].transform.rotation = CloneCollidedObjs [i].transform.rotation;

					if (ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0] && ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0]) {
						if (PairedPortalSkybox && ObjCloneCollidedCamObj [ObjCloneCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> () && !ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> ())
							ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].AddComponent<Skybox> ();
						if (ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> ())
							ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].GetComponent<Skybox> ().material = PairedPortalSkybox;

						ObjCollidedCamObj [ObjCollidedCamObj [1] ? 1 : 0].GetComponent<Camera> ().enabled = true;

						if (PortalFunctionality.SceneAsyncLoad.Enable)
							StartCoroutine ("LoadNextScene");
					}
					if (CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ())
						CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_MouseLook.Init (CollidedObjs [i].transform, CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_Camera.transform);

					if (CollidedObjVelocity [i] != new Vector3 (0, 0, 0))
						CollidedObjs [i].GetComponent<Rigidbody> ().velocity = CollidedObjVelocity [i];
				}

				CollidedObjs [i].name = CollidedObjsInitName [i];

				if (CollidedObjs [i].GetComponent<MeshRenderer> () && PortalSettings.EnableClipPlane && StandardObjShader [i]) {
					CollidedObjs [i].GetComponent<MeshRenderer> ().sharedMaterials = CollidedObjsInitMaterials [i].Materials;

					CollidedObjsInitMaterials [i].Materials = null;
				}

				CollidedObjs [i] = null;
				CollidedObjsInitName [i] = "";
				CollidedObjsAlwaysTeleport [i] = false;
				CollidedObjsFirstTrig [i] = false;
				CollidedObjsFirstTrigDist [i] = 0;
				Destroy (ProxDetCollidedObjs [i]);
				Destroy (CloneCollidedObjs [i]);
				CollidedObjVelocity [i] = new Vector3 (0, 0, 0);
				ContinueTriggerEvents [i] = false;
			}
		}

		if (collision.transform.childCount == 0 && collision.GetComponent<Camera> ()) {
			CollidedObjsExternalParent = true;
			PairedPortal.GetComponent<PortalManager> ().CollidedObjsExternalParent = true;
		}
		if (collision.transform.childCount > 0 && !collision.GetComponent<Camera> ()) {
			CollidedObjsExternalParent = false;
			PairedPortal.GetComponent<PortalManager> ().CollidedObjsExternalParent = false;
		}

		//Enable collision with objects excluded from trigger during teleport
		if (PortalFunctionality.ExcludedObjsFromTrigger.Length > 0)
			for (int m = 0; m < PortalFunctionality.ExcludedObjsFromTrigger.Length; m++)
				if (collision.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () && !PortalFunctionality.ExcludedObjsFromTrigger [m].OnlyForPortal)
					Physics.IgnoreCollision (collision, PortalFunctionality.ExcludedObjsFromTrigger [m].Obj.GetComponent<Collider> (), false);

		ResetVars (true);
	}

	void ResetVars (bool TriggerExit) {
		bool SetVars = false;

		if (CollidedObjs.Length > 0) {
			if (!TriggerExit) {
				for (int i = 0; i < CollidedObjs.Length; i++) {
					if (CloneCollidedObjs [i])
						Destroy (CloneCollidedObjs [i]);

					if (ProxDetCollidedObjs [i])
						Destroy (ProxDetCollidedObjs [i]);

					if (CollidedObjs [i] && CollidedObjs [i].transform.childCount > 0 && CollidedObjs [i].GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ()) {
						Camera CollObjCam = null;

						for (int j = 0; j < CollidedObjs [i].transform.GetComponentsInChildren<Transform> ().Length; j++)
							if (CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].GetComponent<Camera>())
								CollObjCam = CollidedObjs [i].transform.GetComponentsInChildren<Transform> () [j].GetComponent<Camera> ();

						if (CollObjCam && !CollObjCam.enabled)
							CollObjCam.enabled = true;
					}
				}

				SetVars = true;
			}
			if (TriggerExit) {
				int ElementsChecked = 0;

				for (int i = 0; i < CollidedObjs.Length; i++)
					if (!CollidedObjs [i])
						ElementsChecked += 1;

				if (ElementsChecked == CollidedObjs.Length)
					SetVars = true;

				ElementsChecked = 0;
			}
		}

		if (SetVars) {
			CollidedObjs = new GameObject[0];
			CollidedObjsInitName = new string[0];
			CollidedObjsInitMaterials = new InitMaterialsList[0];
			CollidedObjsAlwaysTeleport = new bool[0];
			CollidedObjsFirstTrig = new bool[0];
			CollidedObjsFirstTrigDist = new float[0];
			ProxDetCollidedObjs = new GameObject[0];
			CloneCollidedObjs = new GameObject[0];
			CollidedObjVelocity = new Vector3[0];
			ContinueTriggerEvents = new bool[0];
		}
	}

	IEnumerator LoadNextScene() {
		AsyncOperation AsyncLoad = SceneManager.LoadSceneAsync (PortalFunctionality.SceneAsyncLoad.SceneIndex);

		yield return AsyncLoad;
	}
}