using UnityEngine;
using UnityEngine.ProBuilder;
#if UNITY_EDITOR
using UnityEditor.ProBuilder;
#endif

public class TestProBuilder : MonoBehaviour
{
    void Start()
    {
        // Create a new quad facing forward.
        ProBuilderMesh quad = ProBuilderMesh.Create(
            new Vector3[] {
        new Vector3(0f, 0f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(1f, 1f, 0f)
            },
            new Face[] { new Face(new int[] { 0, 1, 2, 1, 3, 2 } )
        });

        quad.transform.parent = transform;

        // Move one face on the cube along the direction of its normal
        Vertex[] vertices = quad.GetVertices();
        Face face = quad.faces[0];

        Vector3 normal = Math.Normal(quad, face);

        // A face is a collection of triangles, stored in the indexes array. Because mesh geometry requires that seams
        // be inserted at points where normals, UVs, or other vertex attributes differ we use GetCoincidentVertices to
        // collect all vertices at a common position.
        // To see the difference, try replacing `cube.GetCoincidentVertices` with just `face.dinstinctIndexes`.
        foreach (var index in quad.GetCoincidentVertices(face.indexes))
            vertices[index].position += normal;

        quad.SetVertices(vertices);

        // Rebuild the triangle and submesh arrays, and apply vertex positions and submeshes to `MeshFilter.sharedMesh`.
        quad.ToMesh();

        // Recalculate UVs, Normals, Tangents, Collisions, then apply to Unity Mesh.
        quad.Refresh();

        // If in Editor, generate UV2 and collapse duplicate vertices.
#if UNITY_EDITOR
        EditorMeshUtility.Optimize(quad, true);
#else
        // At runtime, `EditorMeshUtility` is not available. To collapse duplicate
        // vertices in runtime, modify the MeshFilter.sharedMesh directly.
        // Note that any subsequent changes to `quad` will overwrite the sharedMesh.
        var umesh = cube.GetComponent<MeshFilter>().sharedMesh;
        MeshUtility.CollapseSharedVertices(umesh);    
#endif
    }
}