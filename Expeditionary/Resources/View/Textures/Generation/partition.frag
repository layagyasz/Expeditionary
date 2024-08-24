#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

uniform float attenuation;
uniform float edge_delta;
uniform vec4 mask;

layout(binding = 0) uniform sampler2D texture0;

void main()
{
    vec3 tex_color = texture(texture0, vert_tex_coord).rgb;
    float edge_dist = min(vert_color.r, min(vert_color.g, vert_color.b));
    vec3 blended = vert_color.rgb +
        3 * attenuation * max(0, edge_dist + edge_delta) * vec3(tex_color.r, tex_color.g, -tex_color.r - tex_color.g);
    blended = normalize(blended);
    blended *= blended;
    vec4 p;
    if (blended.r >= blended.g && blended.r >= blended.b) {
        p = vec4(1, 0, 0, 1);
    }
    else if (blended.g >= blended.b) {
        p = vec4(0, 1, 0, 1);
    }
    else {
        p = vec4(0, 0, 1, 1);
    }
    vec4 result = mask * p;
    out_color = min(0.2f * tex_color.b + 0.9f, 1f) * vec4(1, 1, 1, result.r + result.g + result.b);
}