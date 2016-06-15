// shadertype=glsl
#version 420

layout (location = 0) out vec4 out_color;

layout(binding = 0) uniform sampler2D gPosition;
layout(binding = 1) uniform sampler2D gNormal;
layout(binding = 2) uniform sampler2D gAlbedo;

in vec2 texCoords;

uniform vec3 lightColor = vec3(1,0,0);

vec3 toGamma(vec3 color);

void main() {

	out_color = vec4(texture(gAlbedo, texCoords).xyz * toGamma(lightColor), length(texture(gNormal, texCoords).xyz) == 0 ? 0 : 1);

}