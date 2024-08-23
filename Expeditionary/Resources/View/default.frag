#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

layout(binding = 0) uniform sampler2D texture0;

void main()
{
    out_color = vert_color * texture(texture0, vert_tex_coord / textureSize(texture0, 0));
}