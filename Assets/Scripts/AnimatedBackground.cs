using UnityEngine;

public enum BackgroundType
{
    bg_cave_rock_facets_64,
    bg_concentric_squares_64V1,
    bg_crystal_64,
    bg_flowing_silk_facets_64,
    bg_layered_rock_facets_64,
    bg_organicplates_64,
    bg_papercut_64,
    bg_plates_64,
    bg_polished_stone_facets_64,
    bg_soft_cloud_facets_64,
    bg_softpoly_64,
    bg_tile_64,
    bg_tile_64V1,
    bg_overlapping_plates_64,
    bg_frosted_glass_facets_64_v2,
    bg_frosted_glass_facets_64_v1,
    bg_frosted_glass_overlap_64,
    bg_frosted_menu_planes_64,
    bg_lowpoly_glass_64,
    bg_frosted_planes_64,
    bg_midnight_frosted_glass_64,
    bg_frosted_glass_ambient_64,
    bg_layered_rock_facets_64V1,
    bg_frosted_menu_planes_64V1,
    bg_papercut_64V1,
    bg_tile_64V3,
    bg_concentric_squares_64V2,
    bg_pastel_cream_facets_64,
    bg_pastel_green_facets_64,
    bg_endscene_bridge_64,
    bg_endscene_plates_64
    //Blue, 
    //Brown, 
    //Gray, 
    //Green, 
    //Pink, 
    //Purple, 
    //Yellow
}

public class AnimatedBackground : MonoBehaviour
{
    [SerializeField] private Vector2 movementDirection;
    private MeshRenderer mesh;

    [Header("Color")]
    [SerializeField] private BackgroundType backgroundType;

    [SerializeField] private Texture2D[] textures;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        UpdateBackgroundTexture();
    }

    private void Update()
    {
        mesh.material.mainTextureOffset += movementDirection * Time.deltaTime;
    }

    [ContextMenu("Update Background")]
    private void UpdateBackgroundTexture()
    {
        if (mesh == null)
            mesh = GetComponent<MeshRenderer>();

        mesh.sharedMaterial.mainTexture = textures[(int)(backgroundType)];
    }
}
