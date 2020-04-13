using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour
{
	[Header("Массив Wheel collider")]
	[Tooltip("Передние")]
	public WheelCollider[] WColForward;
	[Tooltip("Задние")]
	public WheelCollider[] WColBack;

	[Header("Массив видимых колёс")]
	[Tooltip("Передние")]
	public Transform[] wheelsF; //1
	[Tooltip("Задние")]
	public Transform[] wheelsB; //1

	[Header("Прочие параметры")]
	public float wheelOffset = 0.1f; //2
	public float wheelRadius = 0.5f; //2

	[Tooltip("Максимальный угол поворота колёс")]
	public float maxSteer = 30f;
	[Tooltip("Ускорение")]
	public float maxAccel = 250f;
	[Tooltip("Торможение")]
	public float maxBrake = 50f;

	[Tooltip("Ссылка на центр массы")]
	public Transform COM;

	public class WheelData
	{
		public Transform wheelTransform;
		public WheelCollider col;
		public Vector3 wheelStartPos;
		public float rotation = 0.0f;
	}

	protected WheelData[] wheels;

	// Use this for initialization
	void Start()
	{
		GetComponent<Rigidbody>().centerOfMass = COM.localPosition;

		wheels = new WheelData[WColForward.Length + WColBack.Length];

		for (int i = 0; i < WColForward.Length; i++)
		{
			wheels[i] = SetupWheels(wheelsF[i], WColForward[i]);
		}

		for (int i = 0; i < WColBack.Length; i++)
		{
			wheels[i + WColForward.Length] = SetupWheels(wheelsB[i], WColBack[i]);
		}

	}

	// Функция SetupWheels() принимает Transform колеса и его WheelCollider, 
	// передает в переменные содержащиеся в классе WheelData необходимые нам данные и возвращает его.
	private WheelData SetupWheels(Transform wheel, WheelCollider col)
	{
		WheelData result = new WheelData();

		result.wheelTransform = wheel;
		result.col = col;
		result.wheelStartPos = wheel.transform.localPosition;

		return result;
	}

	void FixedUpdate()
	{
		float accel = 0;
		float steer = 0;

		accel = Input.GetAxis("Vertical");
		steer = Input.GetAxis("Horizontal");

		CarMove(accel, steer);
		UpdateWheels();
	}

	// движение видимых колёс
	private void UpdateWheels()
	{
		float delta = Time.fixedDeltaTime;

		foreach (WheelData w in wheels)
		{
			WheelHit hit; // в которой в свою очередь содержится точка соприкосновения коллайдера и террейна (переменная point)

			Vector3 lp = w.wheelTransform.localPosition; // локальная позиция колеса
			if (w.col.GetGroundHit(out hit)) // Если WheelCollider колеса сталкивается с поверхностью террейна
			{
				lp.y -= Vector3.Dot(w.wheelTransform.position - hit.point, transform.up) - wheelRadius;
				// То из координаты Y локальной позиции колеса вычитаем Dot() между вектором с началом в точке 
				// в которой коллайдер соприкасается с поверхностью террейна(hit.point) и концом в текущей позиции 
				// колеса(w.wheelTransform.position) и между вектором направленным вверх относительно объекта 
				// Car(transform.up) и из всего этого вычитаем еще и переменную wheelRadius чтобы колесо заняло правильное место
			}
			else
			{
				lp.y = w.wheelStartPos.y - wheelOffset;
				// Если WheelCollider не касается поверхности террейна. То из координаты Y начальной локальной позиции 
				// колеса отнимаем wheelOffset, благодаря этому наши колеса не улетают в неизвестном направлении 
				// когда автомобиль падает с высоты или лежит на «спине»;
			}
			w.wheelTransform.localPosition = lp; // Применяем измененную позицию колеса к его текущей позиции

			w.rotation = Mathf.Repeat(w.rotation + delta * w.col.rpm * 360.0f / 60.0f, 360.0f); // угол «вращения» колеса
			//w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.col.steerAngle, 90.0f); //21
			// Применяем к текущим локальным углам поворота получившийся результат
			w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.col.steerAngle, 0.0f);
		}

	}

	private void CarMove(float accel, float steer)
	{
		foreach (WheelCollider col in WColForward)
		{
			col.steerAngle = steer * maxSteer; // поворот влево-вправо
		}

		if (accel == 0)
		{
			foreach (WheelCollider col in WColBack)
			{
				col.brakeTorque = maxBrake; // brakeTorque - торможение колеса
			}
		}
		else
		{
			foreach (WheelCollider col in WColBack)
			{
				col.brakeTorque = 0;
				col.motorTorque = accel * maxAccel; // motorTorque - крутящий момент колеса.  
			}
		}
	}
}