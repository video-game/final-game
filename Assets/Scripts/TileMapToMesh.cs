#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.AI;

[RequireComponent(typeof(Tilemap))]
public class TileMapToMesh : MonoBehaviour {

    //optional material variable, lets you assign material to the mesh after tilemap is generated.
    public Material material;
    //layerIndex to sat the tileMap mesh to. (camera is then set to ignore this layer)
    [HideInInspector]
    public int layerIndex;

    //It needs a seperate gameobject, as it's parent already has a tilemap renderer and tilemapCollider
    GameObject meshContainer;
    const string containerName = "Mesh Container";

    //reference variables to it's components.
    Tilemap tileMap;
    Grid grid;

    //optional setting to auto apply navMeshSurface and navigation area after tilemap is generated.
    [HideInInspector]
    public bool navMeshModifier;
    [HideInInspector][SerializeField]
    public int navigationArea;

    [HideInInspector]
    public bool meshCollider;
    [HideInInspector]
    public PhysicMaterial physicsMaterial;

    //idiot proofing
    //parent must have be a grid.
    private void Reset()
    {
        //check if TileMapMesh layer exists, if not let the developer know.
        if (LayerMask.NameToLayer("TileMapMesh") == -1)
        {
            Debug.LogWarning("\"TileMapMesh\" Layer does not exist, setting layer to \"Default\".");
        }
        else
        {
            layerIndex = LayerMask.NameToLayer("TileMapMesh");
        }

        //load default material (hardcoded path :S)
        material = AssetDatabase.LoadAssetAtPath("Assets/Material/TileMapMesh.mat", typeof(Material)) as Material;

        if (transform.parent == null && !transform.parent.GetComponentInParent<Grid>())
        {
            Debug.LogError(transform.parent == null ? "GameObject must have a Parent, and the Parent Must have Grid Component" : "Parent Must have Grid Component");
            DestroyImmediate(this);
        }
    }

    //Create the mesh and meshContainer
    public void Generate () {
        //set the references, just incase. (was behaving weird in awake, start and reset)
        tileMap = GetComponent<Tilemap>();
        grid = transform.parent.GetComponentInParent<Grid>();

        CreateGameObject(MeshFromTileMap());
    }
	
    //helper function to create the game object.
    private void CreateGameObject(Mesh mesh)
    {
        //destroy old container if exist
        if (transform.Find(containerName))
        {
            DestroyImmediate(transform.Find(containerName).gameObject);
        }

        meshContainer = new GameObject();
        meshContainer.transform.position = transform.position;
        meshContainer.transform.parent = transform;
        meshContainer.name = containerName;
        MeshFilter mF = meshContainer.AddComponent<MeshFilter>();
        mF.mesh = mesh;
        MeshRenderer mR = meshContainer.AddComponent<MeshRenderer>();

        //add nav Mesh Modifier if exists navMeshModifier = true
        if (navMeshModifier)
        {
            NavMeshModifier nMM = meshContainer.AddComponent<NavMeshModifier>();
            //override the area with navigationArea area type.
            nMM.overrideArea = true;
            nMM.area = navigationArea;
        }

        //apply the material, if set.
        if (material != null)
        {
            mR.material = material;
        }

        meshContainer.layer = layerIndex;

        //add nav Mesh Modifier if exists navMeshModifier = true
        if (meshCollider)
        {
            MeshCollider mC = meshContainer.AddComponent<MeshCollider>();
            mC.sharedMesh = mesh;
            //on a flat mesh, unity gives a convex mesh a width of .05f .. which allows us to hit it with a raycast.
            //nvm .. this causes other issues.
            //mC.convex = true;
            if(physicsMaterial != null)
            {
                mC.material = physicsMaterial;
            }
        }
    }


    //helper function that generates the mesh from the tile positions.
    private Mesh MeshFromTileMap()
    {
        //each tile is a quad made up of 2 triangles. each triangle has 3 points. ergo.. 6
        const int trianglesPerPoint = 6;

        //get the tile positions
        List<Vector2> tiles = tileMap.GetTilePositions<TileBase>();
        
        //generate vertex positions from tile positions.
        //each point has 4 vertices (corners of the quad)
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < tiles.Count; i++)
        {
            points.Add(new Vector2(tiles[i].x, tiles[i].y));
            points.Add(new Vector2(tiles[i].x + grid.cellSize.x, tiles[i].y));
            points.Add(new Vector2(tiles[i].x, tiles[i].y + grid.cellSize.y));
            points.Add(new Vector2(tiles[i].x + grid.cellSize.x, tiles[i].y + grid.cellSize.y));
        }

        //create arrays for the vertices, uvs and triangles.
        Vector3[] vertices = new Vector3[points.Count + 1];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(points.Count) * trianglesPerPoint];

        for (int i = 0; i < points.Count; i++)
        {
            //set the vertex position, using xz
            vertices[i] = new Vector3(points[i].x, 0, points[i].y);

            //every points has 4 vertices.
            //we only run for every 4th point, else we'd have overlaps.
            if (i % 4 == 0)
            {
                //set uvs of the vertices for texture/shader (though, we're not using it .. good practice)
                uvs[i] = new Vector2(0, 0);
                uvs[i+1] = new Vector2(1, 0);
                uvs[i + 2] = new Vector2(0, 1);
                uvs[i + 3] = new Vector2(1, 1);

                //create the polygons
                //lower tile.
                triangles[i * trianglesPerPoint] = i;
                triangles[(i * trianglesPerPoint) + 1] = i + 2;
                triangles[(i * trianglesPerPoint) + 2] = i + 1;

                //upper tile.
                triangles[(i * trianglesPerPoint) + 3] = i + 2;
                triangles[(i * trianglesPerPoint) + 4] = i + 3;
                triangles[(i * trianglesPerPoint) + 5] = i + 1;   
                
            }

        }

        //put it all together.
        Mesh mesh = new Mesh()
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();

        return mesh;
    }
}


//Extension to TileMap that lets you get the positions of all the tiles in the tilemap.
//modified version GetTiles by: Dmitry Tabakov (https://gamedev.stackexchange.com/questions/150917/how-to-get-all-tiles-from-a-tilemap)
public static class TilemapExtensions
{
    public static List<Vector2> GetTilePositions<T>(this Tilemap tilemap) where T : TileBase
    {
        List<Vector2> tiles = new List<Vector2>();

        for (int y = tilemap.origin.y; y < (tilemap.origin.y + tilemap.size.y); y++)
        {
            for (int x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
            {
                T tile = tilemap.GetTile<T>(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    tiles.Add(new Vector2(x, y));
                }
            }
        }
        return tiles;
    }
}
#endif