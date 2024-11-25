#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

uniform float magnitude;
uniform float edge_delta;
uniform vec4 mask;

layout(binding = 0) uniform sampler2D texture0;

void main()
{
    vec3 tex_color = texture(texture0, vert_tex_coord).rgb;
    float edge_dist = 3 * max(0, min(vert_color.r, min(vert_color.g, vert_color.b)) + edge_delta);
    vec3 blended = vert_color.rgb +
        3 * magnitude * max(0, edge_dist + edge_delta) * vec3(tex_color.r, tex_color.g, -tex_color.r - tex_color.g);
    float value = step(1 - dot(mask.rgb, blended.rgb), tex_color.b);
    out_color = vec4(1, 1, 1, value);
}