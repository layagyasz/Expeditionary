#version 430 core

layout(location = 0) in vec3 in_position;  
layout(location = 1) in vec4 in_color;
layout(location = 2) in vec2 in_tex_coord;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec2 vert_position;
out vec4 vert_color;
out vec2 vert_tex_coord;

void main(void)
{
	gl_Position = vec4(in_position, 1.0) * model * view * projection;
	vert_position = 0.5f * (gl_Position.xy / gl_Position.w) + vec2(0.5f, 0.5f);
	vert_color = in_color;
	vert_tex_coord = in_tex_coord;
}