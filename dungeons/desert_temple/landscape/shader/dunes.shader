shader_type spatial;

// Global constants.
// A vector which points directly up. Note that it may be necessary to transform this into the appropriate space.
const vec3 VECTOR_UP = vec3(0, 1, 0);

// Global functions.
// Multiply a 4x4 matrix by a 3-vector in a common way.
// The padding refers to the fourth element which is added to the 3-vector to make it a 4-vector. This typically corresponds to including displacement along with transformation. 
vec3 mat4vec3mult(mat4 matrix, vec3 vector, float padding) {
	return (matrix * vec4(vector, padding)).xyz;
}

// Convert the unsigned component of a vector to a signed one.
// Deals with only floats, which is necessary for this purpose.
float unsignedVectorComponentToSignedVectorComponent(float component) {
	return (component > 0.5) ? -component + 0.5 : component;
}

// Converts an entire vector from unsigned to signed using the unsignedVectorComponentToSignedVectorComponent() function.
vec3 unsignedVectorToSignedVector(vec3 inputVector) {
	// The input vector is unsigned but is assumed to be signed.
	vec3 vec;
	vec.x = unsignedVectorComponentToSignedVectorComponent(inputVector.x);
	vec.y = unsignedVectorComponentToSignedVectorComponent(inputVector.y);
	vec.z = unsignedVectorComponentToSignedVectorComponent(inputVector.z);
	return vec;
}

// --- Vertex functions ---

//void vertex() {
//
//}


// --- Fragment functions --- 

uniform sampler2D ripple_noise_soft;
uniform sampler2D ripple_noise_hard;
const float RIPPLE_POWER = 24.0;
const float RIPPLE_STRENGTH = 0.12;
const float RIPPLE_WAVELENGTH = 16.0;
vec3 ripple_perturb(mat4 inv_camera_matrix, vec3 normal, vec3 vertex) {
	vertex /= RIPPLE_WAVELENGTH;
	vec3 vector_up = mat4vec3mult(inv_camera_matrix, VECTOR_UP, 0.0);
	float steepness = pow(dot(normal, vector_up), RIPPLE_POWER);
	vec3 soft_color = unsignedVectorToSignedVector(texture(ripple_noise_soft, vertex.xz).rgb);
	vec3 hard_color = unsignedVectorToSignedVector(texture(ripple_noise_hard, vertex.xz).rgb);
	vec3 ripple_noise_vector = mix(hard_color, soft_color, steepness);
	ripple_noise_vector = mat4vec3mult(inv_camera_matrix, ripple_noise_vector, 0.0);
	return normal + RIPPLE_STRENGTH * ripple_noise_vector;
}

uniform sampler2D sand_noise;
const float SAND_NOISE_STRENGTH = 0.2;
vec3 sand_perturb(mat4 inv_camera_matrix, vec3 normal, vec3 vertex) {
	vec3 sand_noise_vector = unsignedVectorToSignedVector(texture(sand_noise, vertex.xz).rgb);
	sand_noise_vector = mat4vec3mult(inv_camera_matrix, sand_noise_vector, 0.0);
	return normal + SAND_NOISE_STRENGTH * sand_noise_vector;
}

const vec3 BASE_SAND_COLOR = vec3(0.3, 0.24, 0.18);
void fragment() {
	vec3 world_vertex = mat4vec3mult(CAMERA_MATRIX, VERTEX, 1.0);
	
	ALBEDO = BASE_SAND_COLOR;
	
	vec3 normal = NORMAL;
	normal = ripple_perturb(INV_CAMERA_MATRIX, normal, world_vertex);
	normal = sand_perturb(INV_CAMERA_MATRIX, normal, world_vertex);
	NORMAL = normalize(normal);
}


// --- Lighting functions ---

const vec3 DIFFUSE_SHADOW_COLOR = vec3(0.0, 0.0, 0.02);
const vec3 DIFFUSE_LIT_COLOR = vec3(0.4, 0.28, 0.16);
vec3 get_diffuse_color(vec3 normal, vec3 light) {
//	float shade = max(0, dot(normal, light)); // Lambertian reflectance.
	normal.y *= 0.3;
	float shade = max(0.0, 4.0 * dot(normal, light)); // Journey's "diffuse contrast" reflectance
	return mix(DIFFUSE_SHADOW_COLOR, DIFFUSE_LIT_COLOR, shade);
}

const float RIM_POWER = 16.0;
const vec3 RIM_COLOR = vec3(0.15, 0.1, 0.05);
vec3 get_rim_color(vec3 normal, vec3 view) {
	// Fresnel reflectance.
	float rim = 1.0 - max(0.0, dot(normal, view)); 
	return pow(rim, RIM_POWER) * RIM_COLOR;
}

const float OCEAN_POWER = 16.0;
const vec3 OCEAN_COLOR = vec3(0.4, 0.3, 0.2);
vec3 get_ocean_color(vec3 normal, vec3 view, vec3 light) {
	vec3 half_vector = normalize(view + light);
	float oceanness = pow(max(0, dot(normal, half_vector)), OCEAN_POWER);
	return oceanness * OCEAN_COLOR;
}

void light() {
	vec3 world_light = mat4vec3mult(CAMERA_MATRIX, LIGHT, 0.0);
	vec3 world_normal = mat4vec3mult(CAMERA_MATRIX, NORMAL, 0.0);
	
	vec3 diffuse_color = get_diffuse_color(world_normal, world_light);
	vec3 rim_color = get_rim_color(NORMAL, VIEW);
	vec3 ocean_color = get_ocean_color(NORMAL, VIEW, LIGHT);

	vec3 specular_color = max(rim_color, ocean_color);
	
	DIFFUSE_LIGHT += diffuse_color;
	SPECULAR_LIGHT += specular_color;
}


