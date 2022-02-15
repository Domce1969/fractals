#pragma once

#ifdef GENERATORLIB_EXPORTS
#define GENERATORLIB_API __declspec(dllexport)
#else
#define GENERATORLIB_API __declspec(dllimport)
#endif

struct complex_t;

extern "C" GENERATORLIB_API void __cdecl generator_init(int32_t width, int32_t height, double compare_tolerance);
extern "C" GENERATORLIB_API void __cdecl generator_load_polynomial(int32_t* coefficients, int32_t* powers, int32_t len);
extern "C" GENERATORLIB_API void __cdecl generator_generate(double ax, double ay, double cx, double cy, int32_t accuracy);
extern "C" GENERATORLIB_API int32_t** __cdecl generator_read_results();

bool compare_doubles(double a, double b);
bool compare_complex(const complex_t& a, const complex_t& b);

complex_t eval_polynomial(const complex_t& c);
complex_t eval_polynomial_derivative(const complex_t& c);
complex_t apply_iteration(const complex_t& c);
complex_t complex_pow(const complex_t& c, int power);
complex_t complex_mult_scalar(const complex_t& a, int32_t scalar);
complex_t complex_add(const complex_t& a, const complex_t& b);
int32_t generate_pixel(double x, double y, int32_t iterations);
