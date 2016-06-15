// shadertype=glsl
#version 420

layout (location = 0) out vec4 out_color;

layout(binding = 0) uniform sampler2D gPosition;
layout(binding = 1) uniform sampler2D gNormal;
layout(binding = 2) uniform sampler2D gAlbedo;
layout(binding = 3) uniform sampler2D gLight;

in vec2 texCoords;
flat in int id;

uniform vec3 viewPos;

struct Light{
	vec3 direction;
	float dummy;
	vec3 color;
	float dummy2;
};

layout(std140) uniform Chars {
    Light lights[128];
};

vec3 toGamma(vec3 color);

void main() {

	Light l = lights[id];

	vec3 normal = texture(gNormal, texCoords).xyz;
	vec3 position = texture(gPosition, texCoords).xyz;
	vec3 color = texture(gAlbedo, texCoords).xyz;
	float reflectivity = texture(gLight, texCoords).y;
	float specularity = texture(gLight, texCoords).x * 256.0;

    float diffuse = max(dot(normal, -l.direction), 0.0);
    float specular= reflectivity * max(pow(max(dot(normal, normalize(normalize(-l.direction) + normalize(viewPos - position))), 0.0), specularity),0.0);

	out_color = vec4(toGamma(l.color) * (color * diffuse + toGamma(vec3(1,1,1)) * specular), 1);
	
}