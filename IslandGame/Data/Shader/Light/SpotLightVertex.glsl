// shadertype=glsl
#version 400

const vec3[8] vertices = vec3[8](vec3(-1.0, -1.0,  1.0),
								 vec3( 1.0, -1.0,  1.0),
								 vec3( 1.0,  1.0,  1.0),
								 vec3(-1.0,  1.0,  1.0),
								 vec3(-1.0, -1.0, -1.0),
								 vec3( 1.0, -1.0, -1.0),
								 vec3( 1.0,  1.0, -1.0),
								 vec3(-1.0,  1.0, -1.0));

const int[36] indices = int[36](// front
                                1, 0, 2,
                                3, 2, 0,
                                // top
                                5, 1, 6,
                                2, 6, 1,
                                // back
                                6, 7, 5,
                                4, 5, 7,
                                // bottom
                                0, 4, 3,
                                7, 3, 4,
                                // left
                                5, 4, 1,
                                0, 1, 4,
                                // right
                                2, 3, 6,
                                7, 6, 3);
struct Light{
	vec3 position;
	float radius;
	vec3 color;
	float constant;
	float linear;
	float quadratic;
	float innercutoff;
	float outercutoff;
	vec3 direction;
};

out vec2 texCoords;
out Light light;

uniform mat4 mvp;

layout(std140) uniform Chars {
    Light lights[1024];
};

void main() {

	Light l = lights[gl_InstanceID];

	light = l;

	vec4 screenpos = mvp * vec4(vertices[indices[gl_VertexID]] * l.radius * 1.1 + l.position,1);

    gl_Position = screenpos;
}
