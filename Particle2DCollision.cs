using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Particle2DCollision : MonoBehaviour
{
	private ParticleSystem ps;

	private ParticleSystem.Particle[] particles;

	private RaycastHit2D[] castBuffer = new RaycastHit2D[1];

	public LayerMask collisionMask;

	public LayerMask boundsMask;

	public float checkRadius = 0.05f;

	public float bounceDamping = 0.5f;

	public bool killOnCollision;

	public bool enableDebug = true;

	public bool deleteOutsideBounds = true;

	private void Awake()
	{
		ps = GetComponent<ParticleSystem>();
	}

	private void LateUpdate()
	{
		if (particles == null || particles.Length < ps.main.maxParticles)
		{
			particles = new ParticleSystem.Particle[ps.main.maxParticles];
		}
		int num = ps.GetParticles(particles);
		int layerMask = ((collisionMask.value != 0) ? collisionMask.value : (-1));
		ParticleSystem.MainModule main = ps.main;
		for (int i = 0; i < num; i++)
		{
			Vector3 position = particles[i].position;
			Vector3 vector = ToWorldPosition(position, main.simulationSpace, main.customSimulationSpace);
			Vector2 vector2 = new Vector2(vector.x, vector.y);
			Vector3 velocity = particles[i].velocity;
			Vector3 vector3 = ToWorldVector(velocity, main.simulationSpace, main.customSimulationSpace);
			Vector2 vector4 = new Vector2(vector3.x, vector3.y);
			Vector2 vector5 = vector2 - vector4 * Time.deltaTime;
			Vector2 vector6 = vector2 - vector5;
			float magnitude = vector6.magnitude;
			Vector2 direction = ((magnitude > 0f) ? (vector6 / magnitude) : Vector2.zero);
			RaycastHit2D raycastHit2D = default(RaycastHit2D);
			if (magnitude > 0f)
			{
				raycastHit2D = Physics2D.CircleCast(vector5, Mathf.Max(0.0001f, checkRadius), direction, magnitude, layerMask);
			}
			if (deleteOutsideBounds && Physics2D.OverlapCircle(vector2, checkRadius, boundsMask) == null)
			{
				particles[i].remainingLifetime = -1f;
				particles[i].startSize = 0f;
			}
			if (raycastHit2D.collider != null)
			{
				if (killOnCollision)
				{
					particles[i].remainingLifetime = -1f;
					particles[i].startSize = 0f;
					continue;
				}
				Vector2 vector7 = Vector2.Reflect(vector4, raycastHit2D.normal) * bounceDamping;
				Vector3 worldVector = new Vector3(vector7.x, vector7.y, vector3.z);
				Vector2 vector8 = raycastHit2D.point + raycastHit2D.normal * checkRadius;
				Vector3 worldPosition = new Vector3(vector8.x, vector8.y, vector.z);
				particles[i].velocity = ToSimVector(worldVector, main.simulationSpace, main.customSimulationSpace);
				particles[i].position = ToSimPosition(worldPosition, main.simulationSpace, main.customSimulationSpace);
			}
		}
		ps.SetParticles(particles, num);
	}

	private Vector3 ToWorldPosition(Vector3 simPosition, ParticleSystemSimulationSpace space, Transform customSpace)
	{
		switch (space)
		{
		case ParticleSystemSimulationSpace.World:
			return simPosition;
		case ParticleSystemSimulationSpace.Local:
			return base.transform.TransformPoint(simPosition);
		case ParticleSystemSimulationSpace.Custom:
			if (!(customSpace != null))
			{
				return base.transform.TransformPoint(simPosition);
			}
			return customSpace.TransformPoint(simPosition);
		default:
			return simPosition;
		}
	}

	private Vector3 ToWorldVector(Vector3 simVector, ParticleSystemSimulationSpace space, Transform customSpace)
	{
		switch (space)
		{
		case ParticleSystemSimulationSpace.World:
			return simVector;
		case ParticleSystemSimulationSpace.Local:
			return base.transform.TransformVector(simVector);
		case ParticleSystemSimulationSpace.Custom:
			if (!(customSpace != null))
			{
				return base.transform.TransformVector(simVector);
			}
			return customSpace.TransformVector(simVector);
		default:
			return simVector;
		}
	}

	private Vector3 ToSimPosition(Vector3 worldPosition, ParticleSystemSimulationSpace space, Transform customSpace)
	{
		switch (space)
		{
		case ParticleSystemSimulationSpace.World:
			return worldPosition;
		case ParticleSystemSimulationSpace.Local:
			return base.transform.InverseTransformPoint(worldPosition);
		case ParticleSystemSimulationSpace.Custom:
			if (!(customSpace != null))
			{
				return base.transform.InverseTransformPoint(worldPosition);
			}
			return customSpace.InverseTransformPoint(worldPosition);
		default:
			return worldPosition;
		}
	}

	private Vector3 ToSimVector(Vector3 worldVector, ParticleSystemSimulationSpace space, Transform customSpace)
	{
		switch (space)
		{
		case ParticleSystemSimulationSpace.World:
			return worldVector;
		case ParticleSystemSimulationSpace.Local:
			return base.transform.InverseTransformVector(worldVector);
		case ParticleSystemSimulationSpace.Custom:
			if (!(customSpace != null))
			{
				return base.transform.InverseTransformVector(worldVector);
			}
			return customSpace.InverseTransformVector(worldVector);
		default:
			return worldVector;
		}
	}
}
