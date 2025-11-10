#version 430 core

#define OFFSET vec2(0.5f, 0.5f)

#define ATTENUATION 0.24
#define FREQUENCY vec3(10f, 10f, 10f)
#define LACUNARITY 2f
#define OCTAVES 8
#define PERSISTENCE 0.8f

#define SEED_X 1234
#define SEED_Y 5678

#define BRIGHTNESS 0.8f
#define DISTORTION vec2(0.4f, 0.05f)

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

layout(binding = 0) uniform sampler2D tex_shape;
layout(binding = 1) uniform sampler2D tex_lut;

uniform float time;

const vec3 kernel[] = vec3[]
(
	vec3(1,1,0), vec3(-1,1,0), vec3(1,-1,0), vec3(-1,-1,0),
	vec3(1,0,1), vec3(-1,0,1), vec3(1,0,-1), vec3(-1,0,-1),
	vec3(0,1,1), vec3(0,-1,1), vec3(0,1,-1), vec3(0,-1,-1),
	vec3(1,1,0), vec3(-1,-1,0),  vec3(1,0,-1),  vec3(0,-1,1)
);

uint hash(uint x) 
{
    x += (x << 10u);
    x ^= (x >>  6u);
    x += (x <<  3u);
    x ^= (x >> 11u);
    x += (x << 15u);
    return x;
}

uint hash(int x, int y, int z, int seed) 
{ 
    return hash(x ^ hash(y) ^ hash(z) ^ seed);
}

vec3 get_vector(int x, int y, int z, int seed)
{
	return kernel[hash(x, y, z, seed) & 15];
}

float interpolate(vec3 u)
{
	return (1 - u.x) * (1 - u.y) * (1 - u.z);
}

float octave(vec3 position, int seed) {
	int gx = int(floor(position.x));
	int gy = int(floor(position.y));
	int gz = int(floor(position.z));
	float dx = position.x - gx;
	float dy = position.y - gy;
	float dz = position.z - gz;

	float o = 
		interpolate(vec3(dx, dy, dz)) * dot(vec3(dx, dy, dz), get_vector(gx, gy, gz, seed));
	float p = interpolate(vec3(1 - dx, dy, dz)) * dot(vec3(dx - 1, dy, dz), get_vector(gx + 1, gy, gz, seed));
	float q = interpolate(vec3(dx, 1 - dy, dz)) * dot(vec3(dx, dy - 1, dz), get_vector(gx, gy + 1, gz, seed));
	float r = 
		interpolate(vec3(1 - dx, 1 - dy, dz)) * dot(vec3(dx - 1, dy - 1, dz), get_vector(gx + 1, gy + 1, gz, seed));

	float s = interpolate(vec3(dx, dy, 1 - dz)) * dot(vec3(dx, dy, dz - 1), get_vector(gx, gy, gz + 1, seed));
	float t = 
		interpolate(vec3(1 - dx, dy, 1 - dz)) * dot(vec3(dx - 1, dy, dz - 1), get_vector(gx + 1, gy, gz + 1, seed));
	float u = 
		interpolate(vec3(dx, 1 - dy, 1 - dz)) * dot(vec3(dx, dy - 1, dz - 1), get_vector(gx, gy + 1, gz + 1, seed));
	float v = 
		interpolate(vec3(1 - dx, 1 - dy, 1 - dz))
		* dot(vec3(dx - 1, dy - 1, dz - 1), get_vector(gx + 1, gy + 1, gz + 1, seed));

	return o + p + q + r + s + t + u + v;
}

float noise(vec3 position, int seed) {
	float total = 0;
    float a = 1f;
	vec3 f = FREQUENCY;
    for (int i = 0; i < OCTAVES; ++i)
    {
        total += a * octave(f * position, seed);
        a *= PERSISTENCE;
        f *= LACUNARITY;
    }
    return ATTENUATION * total;
}

void main()
{
	vec3 pos = vec3(vert_tex_coord, time);
	float radius = length(vert_tex_coord - OFFSET);
	if (radius < 1)
	{
		float distortion = dot(DISTORTION, vec2(radius, 1f));
		float noise_x = noise(pos, SEED_X);
		float noise_y = noise(pos, SEED_Y);
		float value = texture(tex_shape, vert_tex_coord + distortion * vec2(noise_x, noise_y)).x;
		out_color = 
			BRIGHTNESS
			* vert_color
			* vec4(texture(tex_lut, vec2(value, clamp(radius, 0f, 1f))).rgb, value);
	}
	else
	{
		out_color = vec4(0,0,0,0);
	}
}