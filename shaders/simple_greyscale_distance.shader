shader_type spatial;
render_mode unshaded;

const float LIGHTENING_RANGE = 2.5;
const float MINIMUM_COLOR = 0.1;
const float MAXIMUM_COLOR = 0.6;

void fragment() {
	float distance_to_vertex = length(VERTEX - VIEW); // r = |v - c| where v - c is a vector pointing from camera to vertex
	ALBEDO = vec3( // make the albedo a greyscale color based on how far away the camera is
		clamp(
			LIGHTENING_RANGE / distance_to_vertex, // A / r
			MINIMUM_COLOR,
			MAXIMUM_COLOR
		)
	);
}