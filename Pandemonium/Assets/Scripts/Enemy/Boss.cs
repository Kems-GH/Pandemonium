using UnityEngine;

public class Boss : Enemy
{
    public override int maxHealth { get; } = 200;
    public override float distanceNearHeart { get; } = 4.1f;
    public override float speedAttack { get; } = 6f;
    public override float timeForFirstAttack { get; } = 1f;
    public override int goldEarnedAfterDeath { get; } = 50;
    public override int damageInflicted { get; } = 15;
    public override int radiusAggro { get;} = 0;
    public override float speed { get;} = 0.6f;
    public override bool ignorePlayer { get; } = true;
}
