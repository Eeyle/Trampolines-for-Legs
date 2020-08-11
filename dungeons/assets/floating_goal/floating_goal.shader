shader_type spatial;
render_mode unshaded;

uniform sampler2D noise;

const vec3 VECTOR_FORWARD = vec3(1.0, 0.0, 0.0);
const vec3 VECTOR_UP = vec3(0.0, 1.0, 0.0);
const vec3 VECTOR_RIGHT = vec3(0.0, 0.0, 0.1);

const float NOISE_SCALE = 0.8;
const float NOISE_BOUNCE_SCALE = 0.03;
const float NOISE_FREQUENCY = 1.0;
const float NOISE_AMPLITUDE = 128.0;

void vertex() {
	// moving noise
	float l = clamp(length(VERTEX.xz), 0.3, 0.6);
	VERTEX.y += NOISE_SCALE * sin(NOISE_FREQUENCY * TIME + NOISE_BOUNCE_SCALE / (l * l)) *
		texture(noise, VERTEX.xz).r; 
}

const float LIGHTENING_RANGE = 10.0;
const float MINIMUM_COLOR = 0.02;
const float MAXIMUM_COLOR = 0.5;

void fragment() {
	float distance_to_vertex = length(VERTEX - VIEW); // r = |v - c| where v - c is a vector pointing from camera to vertex
	ALBEDO = vec3( // make the albedo a greyscale color based on how far away the camera is
		clamp(
			LIGHTENING_RANGE / (distance_to_vertex * distance_to_vertex), // A / r^2
			MINIMUM_COLOR,
			MAXIMUM_COLOR
		)
	);
}