public class BasicSkeleton : Enemy
{
    private void Awake()
    {
        goldEarnedAfterDeath = 5;
        health.Value = 100;
        distanceNearHeart = 2.1f;
        speedAttack = 3.0f;
        timeForFirstAttack = 0.1f;
    }
}
