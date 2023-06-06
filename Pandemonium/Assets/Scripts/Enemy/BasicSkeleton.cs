public class BasicSkeleton : Enemy
{
    public override int maxHealth { get; } = 100;
    public override float distanceNearHeart { get; } = 2.1f;
    public override float speedAttack { get; } = 3f;
    public override float timeForFirstAttack { get; } = 0.1f;
    public override int goldEarnedAfterDeath { get; } = 0;
    public override int damageInflicted { get; } = 5;
    public override int radiusAggro { get;} = 10;
    public override int speed { get;} = 1;
}
