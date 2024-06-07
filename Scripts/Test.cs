
using UnityEngine;
public class test : MonoBehaviour
{
    public GameObject[] objectsToArrange1;
    public GameObject[] objectsToArrange2;
    public GameObject[] objectsToArrange3;
    public GameObject[] objectsToArrange4;

    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    public GameObject obj4;

    public float radius1;
    public float radius2;
    public float radius3;
    public float radius4;

    public float rotSpeed = 10f;

    void Start()
    {
        ArrangeObjectsInCircle();
        obj1.transform.position= new Vector3(0.349999994f,-1.37f,-3.95000005f);
        obj2.transform.position = new Vector3(0.0799999982f,0.680000007f,-3.57999992f);
        obj3.transform.position = new Vector3(0.0799999982f,1.3299998f,-2.26999998f);
        obj4.transform.position = new Vector3(0.0799999982f,2.68000007f,-1.34000003f);
    }

    void Update()
    {
        RotateParent();
    }

    void ArrangeObjectsInCircle()
    {
        ArrangeCircle(objectsToArrange1, 9f, radius1, obj1);
        ArrangeCircle(objectsToArrange2, 5f, radius2, obj2);
        ArrangeCircle(objectsToArrange3, 1f, radius3, obj3);
        ArrangeCircle(objectsToArrange4, -3f, radius4, obj4);
    }

    void ArrangeCircle(GameObject[] objects, float height, float radius, GameObject parent)
    {
        int numberOfObjects = objects.Length;
        float angleStep = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 localPosition = new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius);
            objects[i].transform.SetParent(parent.transform);
            objects[i].transform.localPosition = localPosition;
        }
    }

    void RotateParent()
    {
        obj1.transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
        obj2.transform.Rotate(new Vector3(0, -rotSpeed/2 * Time.deltaTime, 0));
        obj3.transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
        obj4.transform.Rotate(new Vector3(0, -rotSpeed/5 * Time.deltaTime, 0));
    }
}
