#include "pch.h"
#include "generator.h"
#include <cmath>

struct complex_t {
	double real = 0;
	double imag = 0;
};

struct block_t {
	int32_t** roots = nullptr;
	int32_t width = 0;
	int32_t height = 0;
	double compare_tolerance = 0;
};

static struct {
	std::vector<int32_t> coefficients;
	std::vector<int32_t> powers;
	std::vector<complex_t> roots;
	int degree = 0;
} polynomial_t;

block_t* __cdecl allocate_block(int32_t width, int32_t height, double compare_tolerance) {
	block_t* newblock = new block_t();
	newblock->width = width;
	newblock->height = height;
	newblock->compare_tolerance = compare_tolerance;
	newblock->roots = new int32_t*[width];
	for(int i = 0; i < width; i++) {
		newblock->roots[i] = new int32_t[height];
	}

	return newblock;
}

void __cdecl free_block(block_t* block) {
	for(int i = 0; i < block->width; i++) {
		delete[] block->roots[i];
	}
	delete[] block->roots;
	delete block;
}

void __cdecl generator_load_polynomial(int32_t* coefficients, int32_t* powers, int32_t len) {
	polynomial_t.coefficients.clear();
	polynomial_t.powers.clear();
	polynomial_t.roots.clear();
	polynomial_t.degree = 0;

	for(int i = 0; i < len; i++) {
		polynomial_t.coefficients.push_back(coefficients[i]);
		polynomial_t.powers.push_back(powers[i]);
		polynomial_t.degree = max(polynomial_t.degree, powers[i]);
	}

	complex_t zero;
	zero.real = 0;
	zero.imag = 0;
	for(double x = -3; x <= 3; x += 0.01) {
		for(double y = -3; y <= 3; y += 0.01) {
			complex_t c;
			c.real = x;
			c.imag = y;
			for(int i = 0; i < 500; i++) {
				c = apply_iteration(c);
			}

			complex_t evaluated = eval_polynomial(c);
			if(compare_complex(evaluated, zero, 1e-2)) {
				bool contains = false;
				for(const complex_t& r : polynomial_t.roots) {
					if(compare_complex(c, r, 1e-2)) {
						contains = true;
						break;
					}
				}

				if(!contains) {
					polynomial_t.roots.push_back(c);
					if(polynomial_t.roots.size() == polynomial_t.degree) return;
				}
			}
		}
	}
}

void __cdecl generator_generate(double ax, double ay, double bx, double by, int32_t iterations, block_t* block) {
	double stepx = (bx - ax) / block->width;
	double stepy = (by - ay) / block->height;

	for(int i = 0; i < block->width; i++) {
		for(int j = 0; j < block->height; j++) {
			block->roots[i][j] = generate_pixel(ax + i*stepx, ay + j*stepy, iterations, block->compare_tolerance);
		}
	}
}

int32_t** __cdecl generator_read_block(block_t* block, int32_t& width, int32_t& height, double& compare_tolerance) {
	width = block->width;
	height = block->height;
	compare_tolerance = block->compare_tolerance;
	return block->roots;
}

bool compare_doubles(double a, double b, double tolerance) {
	return abs(a - b) <= tolerance;
}

bool compare_complex(const complex_t& a, const complex_t& b, double tolerance) {
	return compare_doubles(a.real, b.real, tolerance) && compare_doubles(a.imag, b.imag, tolerance);
}

complex_t eval_polynomial(const complex_t& c) {
	complex_t ans;
	for(int i = 0; i < polynomial_t.coefficients.size(); i++) {
		ans = complex_add(
			ans,
			complex_mult_scalar(
				complex_pow(c, polynomial_t.powers[i]),
				polynomial_t.coefficients[i]
			)
		);
	}
	return ans;
}

complex_t eval_polynomial_derivative(const complex_t& c) {
	complex_t ans;
	for(int i = 0; i < polynomial_t.coefficients.size(); i++) {
		if(polynomial_t.powers[i] > 0) {
			ans = complex_add(
				ans,
				complex_mult_scalar(
					complex_pow(c, polynomial_t.powers[i] - 1),
					polynomial_t.coefficients[i] * polynomial_t.powers[i]
				)
			);
		}
	}
	return ans;
}

complex_t apply_iteration(const complex_t& c) {
	complex_t a = eval_polynomial(c);
	complex_t b = eval_polynomial_derivative(c);

	double denom = (b.real * b.real + b.imag * b.imag);
	complex_t ans;
	ans.real = c.real - (a.real * b.real + a.imag * b.imag) / denom;
	ans.imag = c.imag - (a.imag * b.real - a.real * b.imag) / denom;
	return ans;
}

// does not work for powers < 0
complex_t complex_pow(const complex_t& c, int power) {
	complex_t ans;
	double newr, newi;
	if(power <= 0) {
		ans.real = 1;
		ans.imag = 0;
	} else {
		ans.real = c.real;
		ans.imag = c.imag;

		for(int i = 1; i < power; i++) {
			newr = (ans.real * c.real - ans.imag * c.imag);
			newi = (ans.real * c.imag + ans.imag * c.real);
			ans.real = newr;
			ans.imag = newi;
		}
	}
	return ans;
}

complex_t complex_mult_scalar(const complex_t& a, int32_t scalar) {
	complex_t ans;
	ans.real = a.real * scalar;
	ans.imag = a.imag * scalar;
	return ans;
}

complex_t complex_add(const complex_t& a, const complex_t& b) {
	complex_t ans;
	ans.real = a.real + b.real;
	ans.imag = a.imag + b.imag;
	return ans;
}

int32_t generate_pixel(double x, double y, int32_t iterations, double tolerance) {
	complex_t c;
	c.real = x;
	c.imag = y;
	for(int i = 0; i < iterations; i++) {
		c = apply_iteration(c);
		for(int j = 0; j < polynomial_t.roots.size(); j++) {
			if(compare_complex(c, polynomial_t.roots[j], tolerance)) {
				return j;
			}
		}
	}
	return -1;
}

