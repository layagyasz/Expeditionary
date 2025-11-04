#version 430 core

#define ARMS 4
#define ROTATION 16
#define SHARPEN 4
#define TAPER 3

#define AMPLITUDE 1
#define FREQUENCY vec3(10f, 10f, 10f)
#define LACUNARITY 2
#define OCTAVES 8
#define OFFSET vec3(2f, 2f, 2f)
#define PERSISTENCE 0.6f

#define SEED_X 1234
#define SEED_Y 5678

#define BRIGHTNESS 0.5f
#define DISTORTION vec2(0.2f, 0.2f)

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;

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
    float a = AMPLITUDE;
    float max = 0;
	vec3 f = FREQUENCY;
    for (int i = 0; i < OCTAVES; ++i)
    {
        total += a * octave(f * position, seed);
        max += a;
        a *= PERSISTENCE;
        f *= LACUNARITY;
    }

    return total / max;
}

float galaxy_density(vec2 position)
{
    float radius = length(position);
    float angle = atan(position.y, position.x);
	float s = SHARPEN * pow(radius, TAPER);
    return (1f - radius * radius) * max(s * cos(ARMS * angle + ROTATION * radius) - s + 1f, 0.5f);
}

void main()
{
	vec3 pos = vec3(vert_tex_coord, 0f);
	float noise_x = noise(OFFSET + pos, SEED_X);
	float noise_y = noise(OFFSET + pos, SEED_Y);
    float density = BRIGHTNESS * galaxy_density(pos.xy + DISTORTION * vec2(noise_x, noise_y));
    out_color = vec4(density, density, density, 1);
}