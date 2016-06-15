// shadertype=glsl
#version 420

layout (location = 0) out vec4 out_color;

layout(binding = 0) uniform sampler2D gPosition;
layout(binding = 1) uniform sampler2D gNormal;
layout(binding = 2) uniform sampler2D gAlbedo;
layout(binding = 3) uniform sampler2D gLight;

struct Light{
	vec3 position;
	float radius;
	vec3 color;
	float constant;
	float linear;
	float quadratic;
};

in vec2 texCoords;
in Light light;

uniform vec3 viewPos;


vec3 toGamma(vec3 color);

void main() {

	vec2 tc = gl_FragCoord.xy/textureSize(gPosition, 0);
	
	vec3 normal = texture(gNormal, tc).xyz;
	vec3 position = texture(gPosition, tc).xyz;
	vec3 color = texture(gAlbedo, tc).xyz;
	float reflectivity = texture(gLight, texCoords).y;
	float specularity = texture(gLight, texCoords).x * 256.0;
	
	float distance    = length(light.position - position);
	
	if(distance < light.radius * 1.1){
		
		float fade = clamp(1+(1 - distance/light.radius)*10, 0, 1);
	
		float attenuation = 1.0f / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
		
		float diffuse = fade * attenuation * max(dot(normal, normalize(light.position - position)), 0.0);
		float specular= fade * attenuation * reflectivity * max(pow(max(dot(normal, normalize(normalize(light.position - position) + normalize(viewPos - position))), 0.0), specularity),0.0);
		
		out_color = vec4(toGamma(light.color.rgb) * (color * diffuse + toGamma(vec3(1,1,1)) * specular), 1);
	}

	//out_color = vec4(1,1,1,1);
}