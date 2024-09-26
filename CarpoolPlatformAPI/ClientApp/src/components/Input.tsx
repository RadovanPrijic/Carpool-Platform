import React, { ChangeEvent } from "react";

interface InputProps {
  label: string;
  id: string;
  name?: string;
  type: string;
  value: string | number;
  onChange: (e: ChangeEvent<HTMLInputElement>) => void;
  placeholder?: string;
  required?: boolean;
  validationErrorMessage?: string;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  [key: string]: any;
}

const Input: React.FC<InputProps> = ({
  label,
  id,
  name,
  type,
  value,
  onChange,
  placeholder,
  required = false,
  validationErrorMessage,
  ...rest
}) => {
  return (
    <div className="input-group">
      <label htmlFor={id}>{label}</label>
      <input
        id={id}
        name={name}
        type={type}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        required={required}
        className="input-field"
        {...rest}
      />
      {validationErrorMessage && <p>{validationErrorMessage}</p>}
    </div>
  );
};

export default Input;
