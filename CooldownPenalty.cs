using System;

[Serializable]
public struct CooldownPenalty
{
	public float expiryTime;

	public float penaltyAmount;

	public CooldownPenalty(float expiry, float penalty)
	{
		expiryTime = expiry;
		penaltyAmount = penalty;
	}
}
