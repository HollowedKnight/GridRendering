using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    public int ColumnCount;
    public int RowCount;

    private Mesh _mesh;
    
    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Procedural Grid";
        _mesh.vertices = GenerateVertices();
        _mesh.uv = GenerateUvs();
        _mesh.triangles = GenerateTriangles();
        _mesh.RecalculateNormals();
        _mesh.tangents = GenerateTangents();
    }

    private Vector3[] GenerateVertices()
    {
        Vector3[] vertices = new Vector3[(ColumnCount + 1) * (RowCount + 1)];
        for (int i = 0, y = 0; y <= RowCount; y++)
        {
            for (int x = 0; x <= ColumnCount; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
            }
        }
        return vertices;
    }
    
    private Vector2[] GenerateUvs()
    {
        Vector2[] uv = new Vector2[_mesh.vertices.Length];
        for (int i = 0, y = 0; y <= RowCount; y++)
        {
            for (int x = 0; x <= ColumnCount; x++, i++)
            {
                uv[i] = new Vector2((float)x / ColumnCount, (float)y / RowCount);
            }
        }
        return uv;
    }
    
    private Vector4[] GenerateTangents()
    {
        Vector4[] tangents = new Vector4[_mesh.vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= RowCount; y++)
        {
            for (int x = 0; x <= ColumnCount; x++, i++)
            {
                tangents[i] = tangent;
            }
        }
        return tangents;
    }
    
    private int[] GenerateTriangles()
    {
        int[] triangles = new int[ColumnCount * RowCount * 6];
        for (int ti = 0, vi = 0, y = 0; y < RowCount; y++, vi++)
        {
            for (int x = 0; x < ColumnCount; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + ColumnCount + 1;
                triangles[ti + 5] = vi + ColumnCount + 2;
            }
        }
        return triangles;
    }

    private void OnDrawGizmos()
    {
        if (_mesh == null || _mesh.vertices == null)
        {
            return;
        }
        
        Gizmos.color = Color.magenta;
        foreach (Vector3 vertex in _mesh.vertices)
        {
            Gizmos.DrawSphere(vertex, 0.1f);
        }
    }
}
