﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTeamButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCreateTeamButton()
    {
        OnCreateTeam?.Invoke();
    }

    public event Action OnCreateTeam ;
}
