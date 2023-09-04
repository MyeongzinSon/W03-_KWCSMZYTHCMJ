using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//using UnityEditor;
using UnityEngine.InputSystem.Utilities;

public class RedBean : Deadly
{
    public RedBeanType type;
    [Header("Linear")]
    [SerializeField, HideInInspector] private LinearType linearType;
    [SerializeField, HideInInspector] private float linearSpeed;
    [SerializeField, HideInInspector] private Vector2 relativeTargetPosition;
    //Only For Repeat
    [SerializeField, HideInInspector] private float interval;

    [Header("Rotational")]
    [SerializeField, HideInInspector] private float rotationSpeed;

    [Header("Circular")]
    [SerializeField, HideInInspector] private float radius = 2f; // The radius of the circular path
    [SerializeField, HideInInspector] private float angularSpeed = 180f; // The speed of rotation in degrees per second
    private Vector3 centerPoint; // The center point of the circular path
    private float angle; // The current angle in degrees

    private Vector2 startingPosition;

    void Start()
    {
        // Calculate the center point of the circular path (to the left by radius)
        centerPoint = transform.position + new Vector3(-radius, 0, 0);
        startingPosition = transform.position;

        switch (type)
        {
            case RedBeanType.None:
                break;
            case RedBeanType.Linear:
                LinearMovement();
                break;
            case RedBeanType.Rotation:
                break;
            case RedBeanType.Circular:
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (type == RedBeanType.Circular) CircularMovement();
        else if (type == RedBeanType.Rotation) RotationMovement();
    }

    void LinearMovement()
    {
        switch (linearType)
        {
            case LinearType.RepeatInstantiate:
                StartCoroutine(StartRepeatInstantiate());
                break;
            case LinearType.Yoyo:
                transform.DOMove((Vector2)transform.position + relativeTargetPosition, Vector2.Distance(transform.position, ((Vector2) transform.position + relativeTargetPosition)) / linearSpeed)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.Flash);
                break;
            default:
                break;
        }
    }

    void RotationMovement()
    {
        // Calculate the rotation amount based on the speed and time
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Rotate the object around its up (Y) axis
        transform.Rotate(Vector3.forward * rotationAmount);
    }

    void CircularMovement()
    {
        // Update the angle based on the speed and time
        angle += angularSpeed * Time.deltaTime;

        // Calculate the new position based on the angle and radius
        float x = centerPoint.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = centerPoint.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        // Set the object's position
        transform.position = new Vector3(x, y, transform.position.z);
    }

    IEnumerator StartRepeatInstantiate()
    {
        Vector2 target = (Vector2)transform.position + relativeTargetPosition;
        bool hasInstantiatedNext = false;
        float instantiateTimer = interval;
        transform.DOMove(target, Vector2.Distance(transform.position, target) / linearSpeed)
            .SetEase(Ease.Flash);

        while (Vector2.Distance(transform.position, target) > Mathf.Epsilon || instantiateTimer > 0)
        {
            if (instantiateTimer <= 0 && hasInstantiatedNext == false)
            {
                hasInstantiatedNext = true;
                Instantiate(gameObject, startingPosition, Quaternion.identity);
            }
            if (Vector2.Distance(transform.position, target) <= Mathf.Epsilon)
            {
                if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;
               if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
            }
            instantiateTimer -= Time.deltaTime;
            yield return null;
        }
        if (hasInstantiatedNext == false)
        {
            hasInstantiatedNext = true;
            GameObject go = Instantiate(gameObject, startingPosition, Quaternion.identity);
            if (go.GetComponent<SpriteRenderer>() != null) go.GetComponent<SpriteRenderer>().enabled = true;
            if (go.GetComponent<Collider2D>() != null) go.GetComponent<Collider2D>().enabled = true;
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        switch (type)
        {
            case RedBeanType.None:
                break;
            case RedBeanType.Linear:
                Gizmos.DrawSphere((Vector2)transform.position + relativeTargetPosition, .1f);
                break;
            case RedBeanType.Rotation:
                break;
            case RedBeanType.Circular:
                Gizmos.DrawSphere(startingPosition + Vector2.left * radius, .1f);
                break;
        }
    }

//#if UNITY_EDITOR
//    [CustomEditor(typeof(RedBean)), CanEditMultipleObjects]
//    public class RedBeanEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();

//            RedBean redBean = (RedBean)target;

//            switch (redBean.type)
//            {
//                case RedBeanType.None:
//                    break;
//                case RedBeanType.Linear:
//                    DrawLinearTypeEditor(redBean);
//                    break;
//                case RedBeanType.Rotation:
//                    DrawRotationTypeEditor(redBean);
//                    break;
//                case RedBeanType.Circular:
//                    DrawCircularTypeEditor(redBean);
//                    break;
//                default:
//                    break;
//            }
//        }

//        static void DrawLinearTypeEditor(RedBean redBean)
//        {
//            redBean.linearType = (LinearType) EditorGUILayout.EnumPopup("Linear Type",redBean.linearType);
//            redBean.linearSpeed = EditorGUILayout.FloatField("Speed", redBean.linearSpeed);
//            redBean.relativeTargetPosition = EditorGUILayout.Vector2Field("Relative Target Position",redBean.relativeTargetPosition);
//            if (redBean.linearType == LinearType.RepeatInstantiate)
//                redBean.interval = EditorGUILayout.FloatField("Interval", redBean.interval);
//        }

//        static void DrawRotationTypeEditor(RedBean redBean)
//        {
//            redBean.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", redBean.rotationSpeed);
//        }

//        static void DrawCircularTypeEditor(RedBean redBean)
//        {
//            redBean.radius = EditorGUILayout.FloatField("Radius", redBean.radius);
//            redBean.angularSpeed = EditorGUILayout.FloatField("Angular Speed", redBean.angularSpeed);

//        }
//    }
//#endif
}

public enum RedBeanType { None, Linear, Rotation, Circular}

public enum LinearType { RepeatInstantiate, Yoyo}
