#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

uniform float attenuation;
uniform vec4 color_a;
uniform vec4 color_b;
uniform vec4 color_c;

layout(binding = 0) uniform sampler2D texture0;

void main()
{
    vec3 tex_color = texture(texture0, vert_tex_coord).rgb;
    float edge_dist = min(vert_color.r, min(vert_color.g, vert_color.b));
    vec3 blended = vert_color.rgb 
        + 3 * attenuation * edge_dist * vec3(tex_color.r, tex_color.g, -tex_color.r - tex_color.g);
    blended = normalize(blended);
    blended *= blended;
    if (blended.r > blended.g && blended.r > blended.b) {
        out_color = color_a;
    }
    else if (blended.g > blended.b) {
        out_color = color_b;
    }
    else {
        out_color = color_c;
    }
    // out_color = blended.r * color_a + blended.g * color_b + blended.b * color_c;
}