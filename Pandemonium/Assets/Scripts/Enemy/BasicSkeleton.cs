using Unity.Netcode;

public class BasicSkeleton : Enemy
{
    protected override int goldEarnedAfterDeath { get; } = 5;
}