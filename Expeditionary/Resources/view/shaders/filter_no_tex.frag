#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

uniform vec4 filter_color;

void main()
{
    out_color = filter_color * vert_color;
}