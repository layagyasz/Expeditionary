#version 430 core

#define EPSILON 0

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

uniform float gauge;
uniform float magnitude;
uniform float edge_delta;
uniform int mask;

layout(binding = 0) uniform sampler2D texture0;

int min_component(vec3 pos)
{
    if (pos.z < pos.y && pos.z < pos.x)
    {
        return 1;
    }
    if (pos.y < pos.x)
    {
        return 2;
    }
    return 4;
}

float get_dist(vec3 pos, int from_component)
{
    if (from_component == 1)
    {
        return abs(pos.x - pos.y);
    }
    if (from_component == 2)
    {
        return abs(pos.x - pos.z);
    }
    return abs(pos.y - pos.z);
}

void main()
{
    vec3 tex_color = texture(texture0, vert_tex_coord).rgb;
    float edge_dist = edge_delta + min(vert_color.r, min(vert_color.g, vert_color.b));
    vec3 blended = vert_color.rgb +
        3 * magnitude * max(0, edge_dist + edge_delta) * vec3(tex_color.r, tex_color.g, -tex_color.r - tex_color.g);
    blended = normalize(blended);
    blended *= blended;
    int min_component = min_component(blended.xyz);
    float cross_dist = 2 * get_dist(blended.xyz, min_component);
    float value = 1 - gauge * cross_dist * cross_dist;
    value = int(2 * value);
    float noise = min(0.2f * tex_color.b + 0.9f, 1f);
    out_color = ((mask & min_component) > 0 ? 1 : 0) * vec4(noise, noise, noise, value);
}