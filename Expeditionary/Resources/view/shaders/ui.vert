#version 430 core

layout(location = 0) in vec3 in_position;  
layout(location = 1) in vec4 in_color;
layout(location = 2) in vec2 in_tex_coord;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec4 vert_color;
out vec2 vert_tex_coord;
out vec3 vert_internal_coord;

void main(void)
{
	gl_Position = vec4(in_position, 1.0) * model * view * projection;
	vert_color = in_color;
	vert_tex_coord = in_tex_coord;
	vert_internal_coord = in_position;
}