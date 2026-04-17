using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathDebbuger;
using CustomMath;
using System;
public class Tester : MonoBehaviour
{
    void Start()
    {
        List<Vector3> vectors = new List<Vector3>();
        vectors.Add(new Vec3(10.0f, 0.0f, 0.0f));
        vectors.Add(new Vec3(10.0f, 10.0f, 0.0f));
        vectors.Add(new Vec3(20.0f, 10.0f, 0.0f));
        vectors.Add(new Vec3(20.0f, 20.0f, 0.0f));
        Vector3Debugger.AddVectorsSecuence(vectors, false, Color.red, "secuencia");
        Vector3Debugger.EnableEditorView("secuencia");
        Vector3Debugger.AddVector(new Vector3(10, 10, 0), Color.blue, "elAzul");
        Vector3Debugger.EnableEditorView("elAzul");
        Vector3Debugger.AddVector(Vector3.down * 7, Color.green, "elVerde");
        Vector3Debugger.EnableEditorView("elVerde");

        Vector3 a = new Vector3(3, 3, 4);
        Vector3 normal = new Vector3(1, 1, 1);

        Vec3 a2 = new Vec3(3, 3, 4);
        Vec3 normal2 = new Vec3(1, 1, 1);

        Vector3 unityProj = Vector3.Project(a, normal);
        Vec3 myProj = Vec3.Project(a2, normal2);

        Console.WriteLine(unityProj);
        Console.WriteLine(myProj);
}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Vector3Debugger.TurnOffVector("elAzul");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3Debugger.TurnOnVector("elAzul");
        }
    }

    IEnumerator UpdateVector()
    {
        for (int i = 0; i < 100; i++)
        {
            //Vector3Debugger.UpdatePosition("elAzul", new Vector3(2.4f, 6.3f, 0.5f) * (i * 0.05f));
            yield return new WaitForSeconds(0.2f);
        }
    }

}
