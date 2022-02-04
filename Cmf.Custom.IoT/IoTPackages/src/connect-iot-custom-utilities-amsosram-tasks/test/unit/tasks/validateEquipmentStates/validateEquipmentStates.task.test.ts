import "reflect-metadata";
import { Task, System, TYPES, DI } from "@criticalmanufacturing/connect-iot-controller-engine";
import EngineTestSuite from "@criticalmanufacturing/connect-iot-controller-engine/test";

// import { DataStoreMock } from "@criticalmanufacturing/connect-iot-controller-engine/test/mocks/dataStore.mock";
import * as chai from "chai";

import {
    ValidateEquipmentStatesTask,
    ValidateEquipmentStatesSettings
} from "../../../../src/tasks/validateEquipmentStates/validateEquipmentStates.task";
import ValidateEquipmentStatesTaskModule from "../../../../src/tasks/validateEquipmentStates/index";

describe("ValidateEquipmentStates Task tests", () => {


    // Optional: See container handling under validateEquipmentStatesTestFactory
    // let dataStoreMock: DataStoreMock;
    beforeEach(() => {
        // dataStoreMock = new DataStoreMock();
    });

    const validateEquipmentStatesTestFactory = (  settings: ValidateEquipmentStatesSettings | undefined,
                                            trigger: Function,
                                            validate: Function): void => {

        const taskDefinition = {
            class: ValidateEquipmentStatesTaskModule,
            id: "0",
            settings: settings || <ValidateEquipmentStatesSettings>{
                message: ""
            }
        };

        EngineTestSuite.createTasks([
            taskDefinition,
            {
                id: "1",
                class: Task.Task({
                    name: "mockTask"
                })(class MockTask implements Task.TaskInstance {
                    [key: string]: any;
                    _outputs: Map<string, Task.Output<any>> = new Map<string, Task.Output<any>>();

                    async onBeforeInit(): Promise<void> {
                        this["activate"] = new Task.Output<any>();
                        this._outputs.set("activate", this["activate"]);
                        // Create other custom outputs (for the Mock task) here
                    }

                    // Trigger the test
                    async onInit(): Promise<void> {
                        trigger(this._outputs);
                    }

                    // Validate the results
                    async onChanges(changes: Task.Changes): Promise<void> {
                        validate(changes);
                    }
                })
            }
        ], [
            { sourceId: "1", outputName: `activate`, targetId: "0", inputName: "activate", },
            { sourceId: "0", outputName: `success`, targetId: "1", inputName: "success", },
            { sourceId: "0", outputName: `error`, targetId: "1", inputName: "error", },
            // Add more links needed here...
        ],
        undefined,
        (containerId) => {
            // Change what you need in the container
            // Example:
            // containerId.unbind(TYPES.System.PersistedDataStore);
            // containerId.bind(TYPES.System.PersistedDataStore).toConstantValue(dataStoreMock);
        });
    };

    /**
     * Instructions about the tests
     * It is assumed that there are two tasks:
     *    0 - ValidateEquipmentStates Task
     *    1 - Mockup task
     *
     * All Outputs of Mock task are connected to the inputs of the ValidateEquipmentStates task
     * All Outputs of ValidateEquipmentStates Task are connected to the Mock task inputs
     *
     * You, as the tester developer, will trigger the outputs necessary for the ValidateEquipmentStates to be activated
     * and check the changes to see if the ValidateEquipmentStates task sent you the correct values
     *
     * Note: This is just an example about how to unit test the task. Not mandatory to use this method!
     */

    it("should get success when activated", (done) => {
        validateEquipmentStatesTestFactory(undefined,
            (outputs: Map<string, Task.Output<any>>) => {
                // Trigger an output
                outputs.get("activate").emit(true);
            }, (changes: Task.Changes) => {
                // Validate the input
                chai.expect(changes["success"].currentValue).to.equal(true);
                // Report the test as a success
                done();
            });
    });
});
