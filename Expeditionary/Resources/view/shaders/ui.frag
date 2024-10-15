#version 430 core

#define MODE_BACKGROUND 0
#define MODE_TEXTURE 1

#define E 0.01f

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;
in vec3 vert_internal_coord;

layout(binding = 0) uniform settings
{
    int textured;
    vec4 foreground_color;
    float border_width[4];
    vec4 border_color[4];
    vec2 corner_radius[4];
};

layout(binding = 0) uniform sampler2D texture0;
uniform vec2 size;

bool outside_ellipse(vec2 point, vec2 radius)
{
    return point.x * point.x / (radius.x * radius.x) + point.y * point.y / (radius.y * radius.y) > 1;
}

int get_corner(vec2 point, vec2 corner_center, vec2 corner_radius, vec2 border_width)
{
    if (border_width.x > corner_radius.x || border_width.y > corner_radius.y)
    {
        return 1;
    }

    vec2 p = point - corner_center;
    if (outside_ellipse(p, corner_radius))
    {
        return -1;
    }
    if (outside_ellipse(p, corner_radius - border_width))
    {
        return 1;
    }
    return 0;
}

int get_border(vec2 pixel_coord)
{
    // Top left corner
    if (pixel_coord.x < corner_radius[0].x && pixel_coord.y < corner_radius[0].y)
    {
        return get_corner(pixel_coord, corner_radius[0], corner_radius[0], vec2(border_width[0], border_width[1]));
    }
    // Top right corner
    if (pixel_coord.x > size.x - corner_radius[1].x && pixel_coord.y < corner_radius[1].y)
    {
        return get_corner(
            pixel_coord, 
            vec2(size.x - corner_radius[1].x, corner_radius[1].y), 
            corner_radius[1], 
            vec2(border_width[2], border_width[1]));
    }
    // Bottom right corner
    if (pixel_coord.x > size.x - corner_radius[2].x && pixel_coord.y > size.y - corner_radius[2].y)
    {
        return get_corner(
            pixel_coord, size - corner_radius[2], corner_radius[2], vec2(border_width[2], border_width[3]));
    }
    // Bottom left corner
    if (pixel_coord.x < corner_radius[3].x && pixel_coord.y > size.y - corner_radius[3].y)
    {
        return get_corner(
            pixel_coord, 
            vec2(corner_radius[3].x, size.y - corner_radius[3].y), 
            corner_radius[3], 
            vec2(border_width[0], border_width[3]));
    }

    if (pixel_coord.x < border_width[0] + E)
    {
        return 1;
    }
    if (pixel_coord.x > size.x - border_width[2])
    {
        return 1;
    }
    if (pixel_coord.y < border_width[1] + E)
    {
        return 1;
    }
    if (pixel_coord.y > size.y - border_width[3])
    {
        return 1;
    }
    return 0;
}

void main()
{
    switch (get_border(vert_internal_coord.xy))
    {
        case -1:
            discard;
            break;
        case 0:
            if (textured == 1)
            {
                ivec2 texture_size = textureSize(texture0, 0);
                vec4 tex_color = foreground_color * texture(texture0, vert_tex_coord / texture_size);
                out_color = 
                    vec4(
                        (1 - tex_color.a) * vert_color.rgb + tex_color.a * tex_color.rgb, 
                        clamp((1 - tex_color.a) * vert_color.a + tex_color.a, 0, 1));
            }
            else
            {   
                out_color = vert_color;
            }
            break;
        case 1:
            out_color = 
                border_color[0] * (1 - vert_tex_coord.x) * (1 - vert_tex_coord.y) +
                border_color[1] * vert_tex_coord.x * (1 - vert_tex_coord.y) +
                border_color[2] * vert_tex_coord.x * vert_tex_coord.y +
                border_color[3] * (1 - vert_tex_coord.x) * vert_tex_coord.y;
                break;
    }
}