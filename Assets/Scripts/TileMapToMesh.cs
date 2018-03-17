using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.AI;

[RequireComponent(typeof(Tilemap), typeof(TilemapCollider2D))]
public class TileMapToMesh : MonoBehaviour {

    //optional material variable, lets you assign material to the mesh after tilemap is generated.
    public Material material;
    //layerName to sat the tileMap mesh to. (camera is then set to ignore this layer)
    public string layerName = "TileMapMesh";

    //It needs a seperate gameobject, as it's parent already has a tilemap renderer and tilemapCollider
    GameObject meshContainer;
    const string containerName = "Mesh Container";

    //reference variables to it's components.
    Tilemap tileMap;
    TilemapCollider2D tileMapCollider;
    Grid grid;

    //optional setting to auto apply navMeshSurface and navigation area after tilemap is generated.
    [HideInInspector]
    public bool navMeshSurface;
    [HideInInspector]
    public string navigationArea = "Walkable";

    //idiot proofing
    //parent must have be a grid.
    private void Reset()
    {
        Debug.Log(transform);
        if(transform.parent == null && !transform.parent.GetComponentInParent<Grid>())
        {
            Debug.LogError(transform.parent == null ? "GameObject must have a Parent, and the Parent Must have Grid Component" : "Parent Must have Grid Component");
            DestroyImmediate(this);
        }
    }

    //Create the mesh and meshContainer
    public void Generate () {
        //set the references, just incase. (was behaving weird in awake, start and reset)
        tileMap = GetComponent<Tilemap>();
        tileMapCollider = GetComponent<TilemapCollider2D>();
        tileMapCollider.enabled = false;
        grid = transform.parent.GetComponentInParent<Grid>();

        CreateGameObject(Composite2DToMesh());
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

        //add nav Mesh Surface if exists navMeshSurface = true
        if (navMeshSurface)
        {
            NavMeshModifier nMM = meshContainer.AddComponent<NavMeshModifier>();
            //override the area with navigationArea area type.
            nMM.overrideArea = true;
            nMM.area = NavMesh.GetAreaFromName(navigationArea);
        }

        //apply the material, if set.
        if (material != null)
        {
            mR.material = material;
        }

        //apply layer if set.
        if (layerName != null)
        {
            //if layer exists use it, else throw error.
            LayerMask layer = LayerMask.NameToLayer(layerName);
            if (LayerMask.LayerToName(layer.value) != string.Empty)
            {
                meshContainer.layer = LayerMask.NameToLayer(layerName);
            }
            else
            {
                Debug.LogWarning(layerName + " layer does not exist");
            }
        }
        
    }


    //helper function that generates the mesh from the tile positions.
    private Mesh Composite2DToMesh()
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

        //get the offset of the collider, just incase.
        Vector2 offset = tileMapCollider.offset;

        //create arrays for the vertices, uvs and triangles.
        Vector3[] vertices = new Vector3[points.Count + 1];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(points.Count) * trianglesPerPoint];

        for (int i = 0; i < points.Count; i++)
        {
            //get the world space location of the points
            Vector2 pt = tileMapCollider.transform.TransformPoint(points[i]);

            //set the vertex position, using xz
            vertices[i] = new Vector3(pt.x + offset.x, 0, pt.y + offset.y);

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
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}


//Extension to TileMap that lets you get the positions of all the tiles in the tilemap.
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
