shader_type canvas_item;

uniform int iterations = 100;
uniform float diverge_limit = 100.0f;
uniform float scale = 8;
uniform vec2 offset = vec2(0.0,0.0);

vec2 Square_Imaginary(vec2 number){
	return vec2( (number.x * number.x ) - (number.y * number.y), (2.0 * number.x * number.y));
}

void fragment(){
	
	vec2 remaped_uv = vec2((UV.x * scale) -scale/2.0 + offset.x, (UV.y * scale) -scale/2.0 + offset.y); // (-2, 2)
	
	float x = remaped_uv.x;
	float y = remaped_uv.y;
	
	x += offset.x;
	y += offset.y;
	
	float  alpha = 1.0;

	vec2 number = vec2(x,y);
	
	for(int i = 0; i < iterations; i++)
	{
		number = Square_Imaginary(number) + vec2(x,y);
	}

	if(number.x * number.x + number.y*number.y < diverge_limit)
	{
		alpha = 0.0;
	}
	
	COLOR = vec4(1.0,1.0,1.0,alpha);
}