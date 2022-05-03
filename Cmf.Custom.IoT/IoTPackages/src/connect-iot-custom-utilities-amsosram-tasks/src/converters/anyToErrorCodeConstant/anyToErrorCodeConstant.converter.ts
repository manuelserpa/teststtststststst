import { Converter, DI, Dependencies, TYPES, Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/anyToErrorCodeConstant.default";
import { CustomErrorCodeEnum } from "../../utilities/customErrorCodeEnum";
import { CustomSystemOfOriginEnum } from "../../utilities/customSystemOfOriginEnum";
/**}
 * @whatItDoes
 *
 * >>TODO: Add description
 *
 */
@Converter.Converter({
    name: i18n.TITLE,
    input: undefined,
    output: Task.TaskValueType.String,
    parameters: {
        ErrorCode: [{
                friendlyName: "CarrierIdFailedToRead",
                value: CustomErrorCodeEnum.CarrierIdFailedToRead
        }, {
            friendlyName: "CarrierFailedToClamp",
            value: CustomErrorCodeEnum.CarrierFailedToClamp
        }, {
            friendlyName: "CarrierFailedToClose",
            value: CustomErrorCodeEnum.CarrierFailedToClose
        }, {
            friendlyName: "CarrierFailedToOpen",
            value: CustomErrorCodeEnum.CarrierFailedToOpen
        }, {
            friendlyName: "CarrierFailedToUnclamp",
            value: CustomErrorCodeEnum.CarrierFailedToUnclamp
        }, {
            friendlyName: "CassetteFailedToLoad",
            value: CustomErrorCodeEnum.CassetteFailedToLoad
        }, {
            friendlyName: "TrackInValidationFailed",
            value: CustomErrorCodeEnum.TrackInValidationFailed
        }, {
            friendlyName: "RecipeValidationFailed",
            value: CustomErrorCodeEnum.RecipeValidationFailed
        }, {
            friendlyName: "FailingTrackIn",
            value: CustomErrorCodeEnum.FailingTrackIn
        }, {
            friendlyName: "CassetteFailedToLoad",
            value: CustomErrorCodeEnum.CassetteFailedToLoad
        }, {
            friendlyName: "CassetteFailedToUnload",
            value: CustomErrorCodeEnum.CassetteFailedToUnload
        }, {
            friendlyName: "CassetteSlotMapFailed",
            value: CustomErrorCodeEnum.CassetteSlotMapFailed
        }, {
            friendlyName: "FailedOnReadyToUnload",
            value: CustomErrorCodeEnum.FailedOnReadyToUnload
        }, {
            friendlyName: "InvalidCarrierDocked",
            value: CustomErrorCodeEnum.InvalidCarrierDocked
        }, {
            friendlyName: "MaterialArrivedError",
            value: CustomErrorCodeEnum.MaterialArrivedError
        }, {
            friendlyName: "MaterialRemovedError",
            value: CustomErrorCodeEnum.MaterialRemovedError
        }, {
            friendlyName: "PPSelectFailed",
            value: CustomErrorCodeEnum.PPSelectFailed
        }, {
            friendlyName: "ProcessError",
            value: CustomErrorCodeEnum.ProcessError
        }, {
            friendlyName: "FatalProcessError",
            value: CustomErrorCodeEnum.FatalProcessError
        }, {
            friendlyName: "WaferProcessError",
            value: CustomErrorCodeEnum.WaferProcessError
        }, {
            friendlyName: "FatalWaferProcessError",
            value: CustomErrorCodeEnum.FatalWaferProcessError
        }, {
            friendlyName: "StartCommandFailed",
            value: CustomErrorCodeEnum.StartCommandFailed
        }, {
            friendlyName: "TrackInFailed",
            value: CustomErrorCodeEnum.TrackInFailed
        }, {
            friendlyName: "OtherError",
            value: CustomErrorCodeEnum.OtherError
        }, {
            friendlyName: "NotifyErrorOnly",
            value: CustomErrorCodeEnum.NotifyErrorOnly
        }],
        SystemOfOrigin: [{
            friendlyName: "EI",
            value: CustomSystemOfOriginEnum.EI
        }, {
            friendlyName: "MES",
            value: CustomSystemOfOriginEnum.MES
        }, {
            friendlyName: "AMHS",
            value: CustomSystemOfOriginEnum.AMHS
        }, {
            friendlyName: "Unknown",
            value: CustomSystemOfOriginEnum.Unknown
        }],
        ErrorNumber: Task.TaskValueType.Integer,
    },
})
export class AnyToErrorCodeConstantConverter implements Converter.ConverterInstance<any, string> {

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    /**
     * >>TODO: Enter description here!
     * @param value any value
     * @param parameters Transformation parameters
     */
    transform(value: any, parameters: { [key: string]: any; }): string {
        return `${parameters["ErrorCode"]}_${parameters["SystemOfOrigin"]}_${parameters["ErrorNumber"]}`;
    }

}
