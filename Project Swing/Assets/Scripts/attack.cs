using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    public int type;
    public int ID;
    public int damage;
    public float push;
    public BoxCollider2D hitbox;
    public FMOD.Studio.EventInstance soundAttackHit;

    public readonly int QUICK = 0, SLOW = 1, HOLD = 2, RAPID = 3;
}

public class Attack1A : Attack
{
    public Attack1A(BoxCollider2D inHitbox)
    {
        type = SLOW;
        ID = 0;
        push = 0.5f;
        damage = 3;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_long");
    }
}

public class Attack1B : Attack
{
    public Attack1B(BoxCollider2D inHitbox)
    {
        type = SLOW;
        ID = 1;
        push = 0.8f;
        damage = 4;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Combo_tackle");
    }
}

public class Attack1C : Attack
{
    public Attack1C(BoxCollider2D inHitbox)
    {
        type = SLOW;
        ID = 2;
        push = 0.5f;
        damage = 5;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Combo_drop_kick");
    }
}

public class Attack2A : Attack
{
    public Attack2A(BoxCollider2D inHitbox)
    {
        type = QUICK;
        ID = 0;
        push = 0f;
        damage = 1;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_short");
    }
}

public class Attack2B : Attack
{
    public Attack2B(BoxCollider2D inHitbox)
    {
        type = QUICK;
        ID = 1;
        push = 0f;
        damage = 1;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_short");
    }
}

public class Attack2C : Attack
{
    public Attack2C(BoxCollider2D inHitbox)
    {
        type = QUICK;
        ID = 2;
        push = 0.5f;
        damage = 3;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Combo_donkey_kick");
    }
}

public class Attack2D : Attack
{
    public Attack2D(BoxCollider2D inHitbox)
    {
        type = QUICK;
        ID = 3;
        push = 0.8f;
        damage = 1;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_short");
    }
}

public class Attack5A : Attack
{
    public Attack5A(BoxCollider2D inHitbox)
    {
        type = HOLD;
        ID = 0;
        push = 0.5f;
        damage = 6;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/HoldAttack");
    }
}

public class Attack6A : Attack
{
    public Attack6A(BoxCollider2D inHitbox)
    {
        type = RAPID;
        ID = 0;
        push = 0.2f;
        damage = 2;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Combo_donkey_kick");
    }
}

public class Attack6B : Attack
{
    public Attack6B(BoxCollider2D inHitbox)
    {
        type = RAPID;
        ID = 1;
        push = 0.3f;
        damage = 3;
        hitbox = inHitbox;
        soundAttackHit = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Combo_donkey_kick");
    }
}