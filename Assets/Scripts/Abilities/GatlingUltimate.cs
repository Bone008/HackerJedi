using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingUltimate : AbstractUltimate
{
    public float activationDuration = 10.0f;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float damageAmount;
    // hacker has to hold triggers down this amount of time
    public float triggerDownTime = 2.0f;
    private float currentTriggerDownTime;
    private bool triggerDown = false;
    private Transform relationObj;
    private float nonVROffset=0;
    private GameObject rotating;
    private float cooldown=0;

    //Verschoben in Oberklasse//private bool activated = false; // if the actual gun has been pulled out

    private void Start()
    {
        try
        {
            relationObj = GameObject.FindGameObjectWithTag("Camera (head)").transform;
        }
        catch(UnityException){
            relationObj = GameObject.FindGameObjectWithTag("Platform").transform;
            nonVROffset = 2;
            Debug.Log("Second_Try");
        }
        Debug.Log("Rel Übergeben");
    }

    private void Update()
    {
        // decrease trigger down timer
        if (!activated && triggerDown)
            currentTriggerDownTime -= Time.deltaTime;

        // pressed long enough -> activate && pay price
        // TODO add gesture?
        if (!activated && triggerDown && currentTriggerDownTime <= 0 && TryConsumeDataFragments())
        {
            SwitchActive(true);

            // deactivate after x seconds
            this.Delayed(activationDuration, () => SwitchActive(false));
        }

        // TODO: In process
        if (activated)
        {
            cooldown -= Time.deltaTime;
            // shoot
            if (cooldown <= 0&&Vector3.Distance(leftHand.position,rightHand.position)<=0.3)
            {
                cooldown = 2;
                GameObject projectile = GameObject.Instantiate(projectilePrefab, transform.position, transform.rotation);//Position muss noch angepasst werden
                projectile.GetComponent<Rigidbody>().velocity = transform.eulerAngles * projectileSpeed;
                Debug.Log("Layer has to be changed!");

                // store damage amount of gun in projectile
                projectile.GetComponent<Projectile>().damageAmount = damageAmount;
            }
        }
    }

    private void LateUpdate()
    {
        // adjust gatling between hands
        if(activated)
        {
            var t = transform.GetChild(0);
            //t.position = leftHand.position;
            //t.rotation = Quaternion.LookRotation(rightHand.position - leftHand.position);
            //t.localScale = new Vector3(0.05f, 0.05f, (rightHand.position - leftHand.position).magnitude * 2);
            //Gatling_Gun an linker Hand positionieren und abhängig von rechter Hand rotieren
            t.position = leftHand.position;
            if (Vector3.Distance(relationObj.position, leftHand.position) > Vector3.Distance(relationObj.position, rightHand.position)/*&&relationObj.position.y-transform.position.y>0.3*/)
            {
                
                //Funktioniert nicht:
                //transform.Translate(Vector3.Slerp(transform.position, leftHand.position - Vector3.up * 0.2f, Time.deltaTime),Space.Self);//anzupassen
                t.transform.localRotation = Quaternion.LookRotation(Vector3.Slerp(leftHand.position - rightHand.position, transform.eulerAngles, Time.deltaTime));
            }
            //if (currentTriggerDownTime <= 0)
            //{
            //    transform.position = leftHand.position - Vector3.up*0.2f;
            //    currentTriggerDownTime += 1;
            //}
            //Laufrotation
            t.transform.GetChild(0).transform.Rotate(new Vector3(3, 0, 0));
        }
    }

    protected override void OnTriggerDown()
    {
        // mark trigger down && start timer
        if (!activated && leftHand.position.y > relationObj.position.y+nonVROffset && rightHand.position.y > relationObj.position.y+nonVROffset)
        {
            //Test auf Abstand zu Head fehlt noch (Umsetzung mit NonVR ist noch schwierig)
            Debug.Log("Geste erkannt");
            triggerDown = true;
            currentTriggerDownTime = triggerDownTime;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    protected override void OnTriggerUp()
    {
        // mark trigger up
        triggerDown = false;
    }

    private void SwitchActive(bool active)
    {
        activated = active;
        transform.GetChild(0).gameObject.SetActive(active);
        triggerDown = active;

        if (active)
            EnableUlti();
        else
        {
            DisableUlti();
            transform.GetChild(0).gameObject.SetActive(false);
        }

        // en/disable old weapons
        // TODO fix and move to AbstractUltimate
        // HackerPlayer hp = hackerPlayer.GetComponentInChildren<HackerPlayer>();
        //hp.GetEquippedAbilityGO(HackerHand.Left).SetActive(!active);
        //hp.GetEquippedAbilityGO(HackerHand.Right).SetActive(!active);
    }

}
