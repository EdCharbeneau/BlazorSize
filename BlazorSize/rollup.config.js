import { terser } from "rollup-plugin-terser";

export default {
    input: "./wwwroot/blazorSize.js",
    output: [
        { file: "./wwwroot/blazorSize.min.js", format: "iife", plugins: [terser()] },
    ]
};