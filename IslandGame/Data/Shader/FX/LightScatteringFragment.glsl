// shadertype=glsl
#version 420

out vec4 color;

layout(binding = 5) uniform sampler2D image;

in vec2 texCoords;

uniform vec3 lightcolor;

vec3 toGamma(vec3 color);

void main() {
 
	color = vec4(toGamma(lightcolor)*texture(image, texCoords).r,1);
}