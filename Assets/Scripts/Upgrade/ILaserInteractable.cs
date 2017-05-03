using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ILaserInteractable
{
    /// <summary>if laser pointer is over it</summary>
    void SetHovered(bool flag);

    /// <summary>selection was confirmed</summary>
    void Activate();
}
