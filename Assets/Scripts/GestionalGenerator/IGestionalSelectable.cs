using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGestionalSelectable
{
    void OnSelect(Action action);
    void OnDeselect(Action action); 
}
