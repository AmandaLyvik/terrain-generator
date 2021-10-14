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

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
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
    }

    private void GenerateMesh() {
        int size = (int)Mathf.Round(Mathf.Pow(2.0f, subdivisions));
        int count = (size + 1) * (size + 1);
        vertices = new Vector3[count];
        uvs = new Vector2[count];
        triangles = new int[6 * size * size];
        
        for (int x = 0; x <= size; x++)
        {
            for (int z = 0; z <= size; z++)
            {
                Vector3 vertex = new Vector3(-scale/2 + x * scale/size, 0.0f, -scale/2 +  z * scale/size);
                Vector2 uv = new Vector2(x / size, z / size);
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

        
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos() {
        if (vertices == null) {
            return;
        }

        for (int i = 0; i < vertices.Length; i++) {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(vertices[i], .1f);   
        }
    }
}
