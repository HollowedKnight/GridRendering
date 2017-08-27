using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProceduralCube : MonoBehaviour
{
    public int XSize;
    public int YSize;
    public int ZSize;
    
    private Mesh _mesh;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "ProceduralCube";
        _mesh.vertices = CreateVertices();
        _mesh.triangles = CreateTriangles();
    }

    private Vector3[] CreateVertices()
    {
        int cornerVertices = 8;
        int edgeVertices = (XSize + YSize + ZSize -3) *4;
        int faceVertices = ((XSize - 1) * (YSize - 1) +
                           (XSize - 1) * (ZSize - 1) +
                           (YSize - 1) * (ZSize - 1)) * 2;
        
        Vector3[] vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
      
        int vertexCount = 0;

        // for each vertical layer
        for (int y = 0; y <= YSize; y++)
        {
            // front left to front right
            for (int x = 0; x <= XSize; x++)
            {
                vertices[vertexCount++] = new Vector3(x, y, 0);
            }

            // front right to back right
            for (int z = 1; z <= ZSize; z++) {
                vertices[vertexCount++] = new Vector3(XSize, y, z);
            }
        
            // back right to back left
            for (int x = XSize -1; x >= 0; x--) {
                vertices[vertexCount++] = new Vector3(x, y, ZSize);
            }
        
            // back left to back right
            for (int z = ZSize -1; z > 0; z--) {
                vertices[vertexCount++] = new Vector3(0, y, z);
            }
        }

        // Top lid
        for (int z = 1; z < ZSize; z++) {
            for (int x = 1; x < XSize; x++) {
                Debug.Log(vertexCount + " /" + vertices.Length);
                vertices[vertexCount++] = new Vector3(x, YSize, z);
            }
        }
        
        // bottom lid
        for (int z = 1; z < ZSize; z++) {
            for (int x = 1; x < XSize; x++) {
                vertices[vertexCount++] = new Vector3(x, 0, z);
            }
        }
        return vertices;
    }

    private int[] CreateTriangles()
    {
        // each quad consists of two triangles
        int quads = (XSize * YSize + XSize * ZSize + YSize * ZSize) * 2;
        // six sides to the cube
        int[] triangles = new int[quads * 6];
        // amount of quads that form one ring
        int ringQuadCount = (XSize + ZSize) * 2;

        int t = 0;
        int v = 0;
        for (int y = 0; y < YSize; y++, v++)
        {
            for (int q = 0; q < ringQuadCount -1; q++, v++) {
                t = SetQuad(triangles, t, v, v + 1, v + ringQuadCount, v + ringQuadCount + 1);
            }
            t = SetQuad(triangles, t, v, v - ringQuadCount +1, v + ringQuadCount, v + 1);
        }

        t = CreateTopFace(triangles, t, ringQuadCount);
        t = CreateBottomFace(triangles, t, ringQuadCount);

        return triangles;
    }

    private int CreateTopFace(int[] triangles, int t, int ringQuadCount)
    {
        int v = ringQuadCount * YSize;
        for (int x = 0; x < XSize - 1; x++, v++) {
            t = SetQuad(triangles, t, v, v + 1, v + ringQuadCount - 1, v + ringQuadCount);
        }
        t = SetQuad(triangles, t, v, v + 1, v + ringQuadCount - 1, v + 2);
		
        int vMin = ringQuadCount * (YSize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < ZSize - 1; z++, vMin--, vMid++, vMax++) {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + XSize - 1);
            for (int x = 1; x < XSize - 1; x++, vMid++) {
                t = SetQuad(
                    triangles, t,
                    vMid, vMid + 1, vMid + XSize - 1, vMid + XSize);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + XSize - 1, vMax + 1);
        }
        int vTop = vMin - 2;
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < XSize - 1; x++, vTop--, vMid++) {
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);
        return t;
    }

    private int CreateBottomFace (int[] triangles, int t, int ring) {
        int v = 1;
        int vMid = _mesh.vertices.Length - (XSize - 1) * (ZSize - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < XSize - 1; x++, v++, vMid++) {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= XSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < ZSize - 1; z++, vMin--, vMid++, vMax++) {
            t = SetQuad(triangles, t, vMin, vMid + XSize - 1, vMin + 1, vMid);
            for (int x = 1; x < XSize - 1; x++, vMid++) {
                t = SetQuad(
                    triangles, t,
                    vMid + XSize - 1, vMid + XSize, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + XSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < XSize - 1; x++, vTop--, vMid++) {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
		
        return t;
    }
    
    private void OnDrawGizmos()
    {
        if (_mesh == null || _mesh.vertices == null)
        {
            return;
        }
        
        Gizmos.color = Color.black;
        foreach (Vector3 vertex in _mesh.vertices)
        {
            Gizmos.DrawSphere(vertex, 0.05f);
        }
    }

    private static int SetQuad(int[] triangles, int index, int v00, int v10, int v01, int v11)
    {
        triangles[index] = v00;
        triangles[index + 1] = triangles[index + 4] = v01;
        triangles[index + 2] = triangles[index + 3] = v10;
        triangles[index + 5] = v11;
        return index + 6;
    }
}