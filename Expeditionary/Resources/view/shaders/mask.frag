#version 430 core

out vec4 out_color;

in vec2 vert_position;
in vec4 vert_color;
in vec2 vert_tex_coord;

layout(binding = 0) uniform sampler2D texture0;
layout(binding = 1) uniform sampler2D texture1;

void main()
{
    out_color = 
        vert_color * texture(texture0, vert_tex_coord / textureSize(texture0, 0)) * texture(texture1, vert_position);
}