#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;
in vec4 gl_FragCoord;

layout(binding = 0) uniform sampler2D mask_tex;

void main()
{
    out_color = vert_color * texture(mask_tex, gl_FragCoord.xy);
}