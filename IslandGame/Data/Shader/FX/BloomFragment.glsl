// shadertype=glsl
#version 420

out vec4 out_color;

layout(binding = 0) uniform sampler2D image;

in vec2 texCoords;

void main() {

	out_color = texture(image, texCoords);
	if(dot(out_color.rgb, vec3(0.2126, 0.7152, 0.0722)) < 1.0)
        out_color = vec4(0,0,0, 1.0);
}