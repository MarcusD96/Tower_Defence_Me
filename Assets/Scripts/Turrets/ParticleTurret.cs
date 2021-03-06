﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTurret : Turret {

    protected FireTurret fireTurret;
    protected WindTurret windTurret;

    [Header("Particle Turret")]
    public ParticleShot shotPrefab;
    protected GameObject currentShot;

    public virtual void Shoot() {
        print("ParticleTurret::AutoShoot");
    }
}
