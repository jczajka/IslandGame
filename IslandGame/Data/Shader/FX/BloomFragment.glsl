// shadertype=glsl
#version 420

out vec4 out_color;

layout(binding = 0) uniform sampler2D image;

in vec2 texCoords;

uniform float bloom_thresh_min = 0.8;
uniform float bloom_thresh_max = 2.5;

void main() {

	vec3 color = texture(image, texCoords).xyz;

	float Y = dot(color, vec3(0.2126, 0.7152, 0.0722));
	color = color * 4.0 * smoothstep(bloom_thresh_min, bloom_thresh_max, Y);
	out_color = vec4(color, 1.0);
	//if(dot(out_color.rgb, vec3(0.299, 0.587, 0.144)) < 1.0)
    //    out_color = vec4(0,0,0, 1.0);
}