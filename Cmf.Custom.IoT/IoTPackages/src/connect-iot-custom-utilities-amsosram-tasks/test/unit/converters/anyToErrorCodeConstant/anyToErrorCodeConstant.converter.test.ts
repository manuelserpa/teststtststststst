import "reflect-metadata";
import { Task, System, TYPES, DI, Converter } from "@criticalmanufacturing/connect-iot-controller-engine";
import EngineTestSuite from "@criticalmanufacturing/connect-iot-controller-engine/test";
import * as chai from "chai";
import { AnyToErrorCodeConstantConverter } from "../../../../src/converters/anyToErrorCodeConstant/anyToErrorCodeConstant.converter";

describe("any to ErrorCode converter", () => {

    let converter: Converter.ConverterContainer;

    beforeEach(async () => {
        converter = await EngineTestSuite.createConverter({
            class: AnyToErrorCodeConstantConverter
        });
    });

    it("should convert", async (done) => {
        /* Example int to string
        let result: string = await converter.execute(123, {
            parameter: "something"
        });

        chai.expect(result).to.equal("123");
        */
        done();
    });

});
