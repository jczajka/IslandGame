// shadertype=glsl
#version 400

const float gamma = 2.2;

vec3 toGamma(vec3 color){
	return pow(color, vec3(gamma));
}

vec3 fromGamma(vec3 color){
	return pow(color, vec3(1.0/gamma));
}