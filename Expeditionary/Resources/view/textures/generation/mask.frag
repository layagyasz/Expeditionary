#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

uniform float edge_delta;
uniform vec4 mask;

layout(binding = 0) uniform sampler2D texture0;

void main()
{
    vec3 tex_color = texture(texture0, vert_tex_coord).rgb;
    float edge_dist = 3 * max(0, min(vert_color.r, min(vert_color.g, vert_color.b)) + edge_delta);
    float value = 
    step(1 - dot(mask.rgb, vert_color.rgb), mix(0, tex_color.r, sqrt(edge_dist)) + 0.5f);
    out_color = min(0.2f * tex_color.b + 0.9f, 1f) * vec4(1, 1, 1, value);
}