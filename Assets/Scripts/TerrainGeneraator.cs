using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]

public class TerrainGeneraator : MonoBehaviour
{
    public float scale = 1.0f;
    [Range(0, 10)]
    public int subdivisions = 3;
    [Range(0.0f, 1.0f)]
    public float heightRatio = 0.1f;
    private int size;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    void Awake()
    {
        mesh = new Mesh();
        size = (int)Mathf.Round(Mathf.Pow(2.0f, subdivisions));
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GenerateMesh();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GenerateMesh();
        ApplyNoise();
        ApplyMesh();
        
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void GenerateMesh() {
        size = (int)Mathf.Round(Mathf.Pow(2.0f, subdivisions));
        int count = (size + 1) * (size + 1);
        vertices = new Vector3[count];
        uvs = new Vector2[count];
        triangles = new int[6 * size * size];
        
        for (int x = 0; x <= size; x++)
        {
            for (int z = 0; z <= size; z++)
            {
                Vector3 vertex = new Vector3(-scale/2 + x * scale/size, 0.0f, -scale/2 +  z * scale/size);
                Vector2 uv = new Vector2(1.0f * x / size, 1.0f * z / size);
                int idx = x + z * (size + 1);
                vertices[idx] = vertex;
                uvs[idx] = uv;
            }
        }

        for (int row = 0; row < size; row++)
        {
            for (int cell = 0; cell < size; cell++)
            {   
                int triIdx = 6 * (cell + row * size);
                int vertBottomLeft = cell + row *(size + 1);
                int vertBottomRight = vertBottomLeft + 1;
                int vertTopLeft = vertBottomLeft + size + 1;
                int vertTopRight = vertTopLeft + 1;

                triangles[triIdx] = vertBottomLeft;
                triangles[triIdx + 1] = vertTopLeft;
                triangles[triIdx + 2] = vertTopRight;

                triangles[triIdx + 3] = vertTopRight;
                triangles[triIdx + 4] = vertBottomRight;
                triangles[triIdx + 5] = vertBottomLeft;
            }
        }
    }

    private void ApplyMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        //mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos() {
        if (vertices == null) {
            return;
        }

        for (int i = 0; i < vertices.Length; i++) {
            Gizmos.color = Color.black;
            //Gizmos.DrawSphere(vertices[i], .1f);   
        }
    }

    private float Noise(float x, float y) {
        float h = 0.0f;

        // Water
        for (int i = 1; i < 10; i++)
        {
            h += 1.0f/i * Mathf.PerlinNoise(x * i + 0.2f*(5-i)*Time.time, y * i - 0.2f*(5-i)*Time.time);
        }


        //h = Mathf.Sin(2 * x * Mathf.PI + Time.time * 2) * Mathf.Cos(2 * y * Mathf.PI);
        return h;
        //return Mathf.Sin(x * z * Mathf.PI + Time.time * 2.0f) * 0.5f + 0.5f;
    }

    private void ApplyNoise() {
        float yMin = 1000000000;
        float yMax = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            Vector2 uv = uvs[i];
            float y = Noise(uv.x, uv.y);
            if (y < yMin) {
                yMin = y;
            }
            if (y > yMax) {
                yMax = y;
            }

            vertices[i] = new Vector3(vertex.x, y * heightRatio * scale, vertex.z);
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            float y0 = vertices[i].y;
            //vertices[i].y = (y0 - (yMin + yMax)/2);
        }
    }
}
